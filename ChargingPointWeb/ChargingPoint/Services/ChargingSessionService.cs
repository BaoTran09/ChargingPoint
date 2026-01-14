using Microsoft.EntityFrameworkCore;
using ChargingPoint.DB;
using ChargingPoint.Models;
using Microsoft.Extensions.Logging;
namespace ChargingPoint.Services
{
 

    public class ChargingSessionService : IChargingSessionService
    {
        private readonly StoreDBContext _context;
        private readonly ILogger<ChargingSessionService> _logger;

        public ChargingSessionService(StoreDBContext context, ILogger<ChargingSessionService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Tạo phiên sạc - startSOC lấy từ IndividualVehicle.BatterySOC
        /// </summary>
        public async Task<ChargingSessionResult> CreateChargingSession(
            long connectorId, string vin, decimal targetSOC, long customerId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _logger.LogInformation($"🔌 Creating session - VIN: {vin}, Target: {targetSOC}%");

                // === 1. LẤY THÔNG TIN XE THỰC (IndividualVehicle) ===
                var individualVehicle = await _context.IndividualVehicles
                    .Include(iv => iv.Vehicle)
                    .Include(iv => iv.Customer)
                    .FirstOrDefaultAsync(v => v.VIN == vin && v.CustomerId == customerId && v.IsActive);

                if (individualVehicle == null)
                    return Fail("Xe không tồn tại hoặc không thuộc về bạn");

                // LẤY STARTSOC TỪ BATTERYSOC CỦA XE
                int currentBatterySOC = individualVehicle.BatterySOC ?? 0;
                if (currentBatterySOC <= 0)
                    return Fail("Không xác định được mức pin hiện tại của xe");

                decimal startSOC = currentBatterySOC;

                // Validation
                if (startSOC >= targetSOC)
                    return Fail($"Mức pin mong muốn ({targetSOC}%) phải lớn hơn mức pin hiện tại ({startSOC}%)");

                if (targetSOC > 100)
                    return Fail("Mức pin tối đa là 100%");

                var vehicleTemplate = individualVehicle.Vehicle;

                // === 2. KIỂM TRA CONNECTOR & CHARGER ===
                var connector = await _context.Connectors
                    .Include(c => c.Charger).ThenInclude(ch => ch.Station)
                    .FirstOrDefaultAsync(c => c.ConnectorId == connectorId);

                if (connector == null)
                    return Fail("Cổng sạc không tồn tại");

                if (connector.Status != "Available")
                    return Fail("Cổng sạc đang được sử dụng");

                if (connector.Charger.Status != "Online")
                    return Fail($"Trụ sạc {connector.Charger.Status}");
                    return Fail($"Trụ sạc {connector.Charger.Status}");

                // === 3. KIỂM TRA TƯƠNG THÍCH ===
                bool isAC = connector.Charger.ChargerType?.Equals("AC", StringComparison.OrdinalIgnoreCase) == true;
                bool isDC = connector.Charger.ChargerType?.Equals("DC", StringComparison.OrdinalIgnoreCase) == true;

                if (isAC && !vehicleTemplate.AcChargingSupport)
                    return Fail("Xe không hỗ trợ sạc AC");
                if (isDC && !vehicleTemplate.DcChargingSupport)
                    return Fail("Xe không hỗ trợ sạc DC");

                // === 4. KIỂM TRA HÓA ĐƠN QUÁ HẠN ===
                bool hasOverdue = await _context.Invoices
                    .AnyAsync(inv => inv.CustomerId == customerId
                                  && inv.Status == "Pending"
                                  && inv.ExpireDate < DateTime.UtcNow);

                if (hasOverdue)
                    return Fail("Bạn có hóa đơn quá hạn. Vui lòng thanh toán trước khi sạc.");

                // === 5. TÍNH CÔNG SUẤT VÀ THỜI GIAN ===
                decimal vehicleMaxPower = isAC
                    ? (vehicleTemplate.MaxAcChargeKW ?? 0)
                    : (vehicleTemplate.MaxDcChargeKW ?? 0);

                decimal chargerMaxPower = connector.Charger.MaxPowerKW ?? 0;

                if (vehicleMaxPower <= 0 || chargerMaxPower <= 0)
                    return Fail("Thông tin công suất không đầy đủ");

                decimal effectivePowerKW = Math.Min(vehicleMaxPower, chargerMaxPower);
                decimal batteryGrossKWh = vehicleTemplate.BatteryGrossKWh ?? 0;

                if (batteryGrossKWh <= 0)
                    return Fail("Thông tin dung lượng pin không đầy đủ");

                // Năng lượng cần sạc
                decimal requiredCapacityKWh = ((targetSOC - startSOC) / 100m) * batteryGrossKWh;

                // Tính thời gian (có thể dùng ChargingCurve nếu có)
                double estimatedSeconds = await CalculateChargingTimeAsync(
                    vehicleTemplate.VehicleId,
                    startSOC,
                    targetSOC,
                    batteryGrossKWh,
                    effectivePowerKW,
                    isDC);

                DateTime now = DateTime.UtcNow;
                DateTime expectTime = now.AddSeconds(estimatedSeconds);

                // === 6. TẠO SESSION ===
                var session = new ChargingSession
                {
                    ConnectorId = connectorId,
                    VIN = vin,
                    VehicleId = vehicleTemplate.VehicleId, // Lưu cả VehicleId (mẫu xe)
                    StartTime = now,
                    ExpectTime = expectTime,
                    StartSOC = startSOC,
                    TargetSOC = targetSOC,
                    EndSOC = null,
                    MeterStartKWh = (startSOC / 100m) * batteryGrossKWh,
                    MeterStopKWh = null,
                    Status = "Charging",
                    LastUpdated = now
                };

                _context.ChargingSessions.Add(session);

                // === 7. CẬP NHẬT STATUS ===
                connector.Status = "InUse";
                // Không cập nhật BatterySOC ở đây, sẽ cập nhật khi hoàn tất

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation($"✓ Session {session.SessionId} created. Start: {startSOC}%, Target: {targetSOC}%, Expect: {expectTime:HH:mm}");

                return new ChargingSessionResult
                {
                    Success = true,
                    Message = "Bắt đầu sạc thành công",
                    SessionId = session.SessionId,
                    ExpectTime = expectTime,
                    SessionStatus = "Charging",
                    EstimatedDurationMinutes = (int)(estimatedSeconds / 60)
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "❌ Error creating session");
                return Fail($"Lỗi: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy tiến độ sạc realtime - tính toán dựa trên StartSOC và thời gian
        /// </summary>
        public async Task<ChargingProgressResult> GetChargingProgress(long sessionId)
        {
            try
            {
                var session = await _context.ChargingSessions
                    .Include(s => s.IndividualVehicle).ThenInclude(iv => iv.Vehicle)
                    .FirstOrDefaultAsync(s => s.SessionId == sessionId);

                if (session == null)
                    return new ChargingProgressResult { Success = false, Message = "Phiên không tồn tại" };

                // Nếu đã kết thúc
                if (session.Status is "Completed" or "Stopped" or "PoweredOff")
                {
                    return new ChargingProgressResult
                    {
                        Success = true,
                        CurrentSOC = (double)(session.EndSOC ?? session.StartSOC ?? 0),
                        Status = session.Status,
                        ProgressPercent = 100,
                        EnergyDeliveredKWh = (double)(session.EnergyDeliveredKWh ?? 0)
                    };
                }

                // === TÍNH TOÁN REALTIME ===
                DateTime now = DateTime.UtcNow;
                DateTime startTime = session.StartTime ?? now;
                DateTime expectTime = session.ExpectTime ?? now.AddHours(1);

                double totalSeconds = (expectTime - startTime).TotalSeconds;
                double elapsedSeconds = (now - startTime).TotalSeconds;

                if (totalSeconds <= 0) totalSeconds = 1;

                double progressRatio = Math.Clamp(elapsedSeconds / totalSeconds, 0, 1);

                decimal startSoc = session.StartSOC ?? 0;
                decimal targetSoc = session.TargetSOC ?? 100;
                double socRange = (double)(targetSoc - startSoc);
                double currentSOC = (double)startSoc + (socRange * progressRatio);

                // Tính năng lượng đã sạc
                decimal batteryGross = session.IndividualVehicle?.Vehicle?.BatteryGrossKWh ?? 0;
                double energyDelivered = ((currentSOC - (double)startSoc) / 100.0) * (double)batteryGross;

                // Kiểm tra xem đã đạt target chưa
                if (currentSOC >= (double)targetSoc)
                {
                    session.Status = "PoweredOff";
                    session.EndSOC = targetSoc;
                    session.PowerOffTime = now;
                    session.MeterStopKWh = (session.MeterStartKWh ?? 0) + ((targetSoc - startSoc) / 100m) * batteryGross;

                    // Cập nhật BatterySOC của xe
                    if (session.IndividualVehicle != null)
                    {
                        session.IndividualVehicle.BatterySOC = (int)targetSoc;
                    }

                    await _context.SaveChangesAsync();

                    // Tự động hoàn tất
                    await CompleteChargingSession(sessionId);

                    currentSOC = (double)targetSoc;
                    progressRatio = 1.0;
                }
                else
                {
                    session.LastUpdated = now;
                    await _context.SaveChangesAsync();
                }

                return new ChargingProgressResult
                {
                    Success = true,
                    CurrentSOC = Math.Round(currentSOC, 1),
                    ProgressPercent = Math.Round(progressRatio * 100, 1),
                    Status = session.Status,
                    ElapsedSeconds = (int)elapsedSeconds,
                    RemainingSeconds = (int)Math.Max(totalSeconds - elapsedSeconds, 0),
                    EnergyDeliveredKWh = Math.Round(energyDelivered, 2)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting progress {sessionId}");
                return new ChargingProgressResult { Success = false, Message = ex.Message };
            }
        }

        /// <summary>
        /// Dừng sạc bởi customer
        /// </summary>
        public async Task<ServiceResult> StopChargingSession(long sessionId)
        {
            try
            {
                var session = await _context.ChargingSessions
                    .Include(s => s.IndividualVehicle).ThenInclude(iv => iv.Vehicle)
                    .Include(s => s.Connector)
                    .FirstOrDefaultAsync(s => s.SessionId == sessionId);

                if (session == null)
                    return new ServiceResult { Success = false, Message = "Phiên không tồn tại" };

                if (session.Status is "Completed" or "Stopped")
                    return new ServiceResult { Success = false, Message = "Phiên đã kết thúc" };

                // Tính SOC hiện tại
                var progress = await GetChargingProgress(sessionId);
                decimal finalSOC = (decimal)progress.CurrentSOC;

                DateTime now = DateTime.UtcNow;
                session.PowerOffTime = now;
                session.EndTime = now;
                session.Status = "Stopped";
                session.EndSOC = finalSOC;

                // Tính năng lượng
                decimal batteryGross = session.IndividualVehicle?.Vehicle?.BatteryGrossKWh ?? 0;
                decimal energyDelivered = ((finalSOC - (session.StartSOC ?? 0)) / 100m) * batteryGross;
                session.MeterStopKWh = (session.MeterStartKWh ?? 0) + energyDelivered;

                // Cập nhật xe
                if (session.IndividualVehicle != null)
                {
                    session.IndividualVehicle.BatterySOC = (int)finalSOC;
                }

                // Giải phóng connector
                if (session.Connector != null)
                {
                    session.Connector.Status = "Available";
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation($"✓ Session {sessionId} stopped at {finalSOC}%");

                return new ServiceResult { Success = true, Message = $"Đã dừng sạc tại {finalSOC:F1}%" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error stopping {sessionId}");
                return new ServiceResult { Success = false, Message = ex.Message };
            }
        }

        /// <summary>
        /// Hoàn tất phiên sạc
        /// </summary>
        public async Task<ServiceResult> CompleteChargingSession(long sessionId)
        {
            try
            {
                var session = await _context.ChargingSessions
                    .Include(s => s.Connector)
                    .Include(s => s.IndividualVehicle).ThenInclude(iv => iv.Vehicle)
                    .FirstOrDefaultAsync(s => s.SessionId == sessionId);

                if (session == null)
                    return new ServiceResult { Success = false, Message = "Phiên không tồn tại" };

                if (session.Status == "Completed")
                    return new ServiceResult { Success = true, Message = "Đã hoàn tất" };

                session.EndTime = DateTime.UtcNow;
                session.Status = "Completed";

                // Giải phóng connector
                if (session.Connector != null)
                {
                    session.Connector.Status = "Available";
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation($"✓ Session {sessionId} completed");

                // TODO: Tạo hóa đơn ở đây
                // await _invoiceService.GenerateInvoiceAsync(sessionId);

                return new ServiceResult { Success = true, Message = "Hoàn tất phiên sạc" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error completing {sessionId}");
                return new ServiceResult { Success = false, Message = ex.Message };
            }
        }

        /// <summary>
        /// Tính thời gian sạc dựa trên ChargingCurve hoặc tuyến tính
        /// </summary>
        private async Task<double> CalculateChargingTimeAsync(
            long vehicleId, decimal startSOC, decimal targetSOC,
            decimal batteryKWh, decimal powerKW, bool isDC)
        {
            if (!isDC)
            {
                // AC: tuyến tính
                decimal energy = ((targetSOC - startSOC) / 100m) * batteryKWh;
                return (double)(energy / powerKW) * 3600;
            }

            // DC: kiểm tra ChargingCurve
            var curves = await _context.ChargingCurves
                .Where(cc => cc.VehicleId == vehicleId && cc.IsDcFastCharge)
                .OrderBy(cc => cc.SocFrom)
                .ToListAsync();

            if (!curves.Any())
            {
                // Không có curve → tuyến tính
                decimal energy = ((targetSOC - startSOC) / 100m) * batteryKWh;
                return (double)(energy / powerKW) * 3600;
            }

            // Tính theo từng đoạn curve
            double totalSeconds = 0;
            foreach (var curve in curves)
            {
                int rangeStart = Math.Max(curve.SocFrom, (int)startSOC);
                int rangeEnd = Math.Min(curve.SocTo, (int)targetSOC);

                if (rangeStart >= rangeEnd) continue;

                decimal socDelta = rangeEnd - rangeStart;
                decimal energyInRange = (socDelta / 100m) * batteryKWh;
                decimal effectivePower = Math.Min(curve.MaxPowerKW, powerKW);

                if (effectivePower <= 0) effectivePower = powerKW;

                totalSeconds += (double)(energyInRange / effectivePower) * 3600;
            }

            return totalSeconds > 0 ? totalSeconds : (double)(((targetSOC - startSOC) / 100m) * batteryKWh / powerKW) * 3600;
        }
        // 5. Bổ sung phương thức ApproveChargingSession (Vì Interface yêu cầu)
        public async Task<ServiceResult> ApproveChargingSession(long sessionId)
        {
            var session = await _context.ChargingSessions.FindAsync(sessionId);
            if (session == null) return new ServiceResult { Success = false, Message = "Không tìm thấy" };
            session.Status = "Charging";
            await _context.SaveChangesAsync();
            return new ServiceResult { Success = true, Message = "Đã phê duyệt" };
        }

        // 6. Bổ sung phương thức RejectChargingSession (Vì Interface yêu cầu)
        public async Task<ServiceResult> RejectChargingSession(long sessionId)
        {
            var session = await _context.ChargingSessions.FindAsync(sessionId);
            if (session == null) return new ServiceResult { Success = false, Message = "Không tìm thấy" };
            session.Status = "Rejected";
            await _context.SaveChangesAsync();
            return new ServiceResult { Success = true, Message = "Đã từ chối" };
        }
        private ChargingSessionResult Fail(string msg) => new() { Success = false, Message = msg };
    }
}