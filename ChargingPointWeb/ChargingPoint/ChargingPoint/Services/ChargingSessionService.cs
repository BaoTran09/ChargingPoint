// *Services/ChargingSessionService.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ChargingPoint.DB;
using ChargingPoint.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ChargingPoint.Services
{
    public class ChargingSessionService : IChargingSessionService
    {
        private readonly StoreDBContext _context;
        private readonly ILogger<ChargingSessionService> _logger;
        private readonly IInvoiceService _invoiceService;
        public ChargingSessionService(
            StoreDBContext context,
            ILogger<ChargingSessionService> logger,
        IInvoiceService invoiceService)
        {
            _context = context;
            _logger = logger;
            _invoiceService = invoiceService;
        }


        /// <summary>
        /// Tạo phiên sạc mới - LOGIC CHUNG cho cả Customer và Admin
        /// </summary>
        public async Task<ChargingSessionResult> CreateChargingSession(
            long connectorId,
            long vehicleId,
            decimal startSOC,
            decimal targetSOC)
        {
            try
            {
                // Validate SOC
                if (startSOC >= targetSOC)
                {
                    return new ChargingSessionResult
                    {
                        Success = false,
                        Message = "Mức pin mong muốn phải lớn hơn mức pin hiện tại"
                    };
                }

                // Lấy thông tin vehicle
                var vehicle = await _context.Vehicle
                    .FirstOrDefaultAsync(v => v.VehicleId == vehicleId);

                if (vehicle == null)
                {
                    return new ChargingSessionResult
                    {
                        Success = false,
                        Message = "Xe không tồn tại"
                    };
                }

                // Lấy thông tin connector
                var connector = await _context.Connector
                    .Include(c => c.Charger)
                    .FirstOrDefaultAsync(c => c.ConnectorId == connectorId);

                if (connector == null)
                {
                    return new ChargingSessionResult
                    {
                        Success = false,
                        Message = "Connector không tồn tại"
                    };
                }

                // Kiểm tra connector availability
                if (connector.Status != "Available")
                {
                    return new ChargingSessionResult
                    {
                        Success = false,
                        Message = "Connector đã được sử dụng"
                    };
                }

                // ============================================
                // ĐIỀU KIỆN 1: Kiểm tra Manufacturer VinFast
                // ============================================
                bool isVinFast = vehicle.Manufacturer?.Equals("VinFast", StringComparison.OrdinalIgnoreCase) == true;
                string sessionStatus = isVinFast ? "Charging" : "Requiring";

                // ============================================
                // ĐIỀU KIỆN 2: Kiểm tra tương thích sạc
                // ============================================
                bool isChargerAC = connector.Charger.ChargerType?.Equals("AC", StringComparison.OrdinalIgnoreCase) == true;
                bool isChargerDC = connector.Charger.ChargerType?.Equals("DC", StringComparison.OrdinalIgnoreCase) == true;

                bool isCompatible = (isChargerAC && vehicle.AcChargingSupport) ||
                                   (isChargerDC && vehicle.DcChargingSupport);

                if (!isCompatible)
                {
                    return new ChargingSessionResult
                    {
                        Success = false,
                        Message = $"Xe không hỗ trợ loại sạc {connector.Charger.ChargerType}. " +
                                 $"AC: {(vehicle.AcChargingSupport ? "Có" : "Không")}, " +
                                 $"DC: {(vehicle.DcChargingSupport ? "Có" : "Không")}"
                    };
                }

                // ============================================
                // TÍNH CÔNG SUẤT X
                // ============================================
                decimal vehicleMaxPower = isChargerAC
                    ? (vehicle.MaxAcChargeKW ?? 0)
                    : (vehicle.MaxDcChargeKW ?? 0);
                decimal chargerMaxPower = connector.Charger.MaxPowerKW ?? 0;
                decimal powerX = Math.Min(vehicleMaxPower, chargerMaxPower);

                if (powerX <= 0)
                {
                    return new ChargingSessionResult
                    {
                        Success = false,
                        Message = "Không xác định được công suất sạc phù hợp"
                    };
                }

                // ============================================
                // TÍNH TOÁN THỜI GIAN SẠC
                // ============================================
                decimal batteryGross = vehicle.BatteryGrossKWh ?? 0;

                if (batteryGross <= 0)
                {
                    return new ChargingSessionResult
                    {
                        Success = false,
                        Message = "Thông tin dung lượng pin xe chưa đầy đủ"
                    };
                }

                // 1. Dung lượng pin hiện tại (kWh): P = StartSOC * (BatteryGrossKWh / 100)
                decimal currentCapacity = (startSOC / 100) * batteryGross;

                // 2. Dung lượng cần sạc (kWh): U = [(TargetSOC - StartSOC) / 100] * BatteryGrossKWh
                decimal requiredCapacity = ((targetSOC - startSOC) / 100) * batteryGross;

                // 3. Thời gian sạc (giây): T = (U / PowerX) * 3600
                decimal timeSeconds = (requiredCapacity / powerX) * 3600;

                // 4. Thời gian ước tính hoàn thành: ExpectTime = StartTime + T
                DateTime startTime = DateTime.Now;
                DateTime expectTime = startTime.AddSeconds((double)timeSeconds);

                // ============================================
                // TẠO CHARGING SESSION
                // ============================================
                var session = new ChargingSession
                {
                    ConnectorId = connectorId,
                    VehicleId = vehicleId,
                    StartTime = startTime,
                    ExpectTime = expectTime,
                    StartSOC = startSOC,
                    TargetSOC = targetSOC,
                    MeterStartKWh = currentCapacity,
                    Status = sessionStatus,
                    LastUpdated = DateTime.Now
                };

                _context.ChargingSession.Add(session);

                // Cập nhật connector status (chỉ khi Charging, không phải Requiring)
                if (sessionStatus == "Charging")
                {
                    connector.Status = "InUse";
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Created charging session {SessionId} for vehicle {VehicleId} with status {Status}",
                    session.SessionId, vehicleId, sessionStatus);

                return new ChargingSessionResult
                {
                    Success = true,
                    Message = isVinFast
                        ? $"Đã bắt đầu sạc! Dự kiến hoàn tất lúc {expectTime:HH:mm:ss}"
                        : "Yêu cầu sạc đã được gửi. Vui lòng chờ nhân viên xác nhận.",
                    SessionId = session.SessionId,
                    SessionStatus = sessionStatus,
                    ExpectTime = expectTime
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating charging session");
                return new ChargingSessionResult
                {
                    Success = false,
                    Message = $"Có lỗi xảy ra: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Lấy tiến trình sạc realtime - LOGIC CHUNG
        /// </summary>
        public async Task<ChargingProgressResult> GetChargingProgress(long sessionId)
        {
            try
            {
                var session = await _context.ChargingSession
                    .Include(s => s.Vehicle)
                    .Include(s => s.Connector)
                        .ThenInclude(c => c.Charger)
                    .FirstOrDefaultAsync(s => s.SessionId == sessionId);

                if (session == null)
                {
                    return new ChargingProgressResult
                    {
                        Success = false,
                        Message = "Session not found"
                    };
                }

                // Nếu không đang charging, trả về SOC cuối
                if (session.Status != "Charging")
                {
                    return new ChargingProgressResult
                    {
                        Success = true,
                        IsCompleted = session.Status == "PoweredOff" || session.Status == "Completed",
                        CurrentSOC = (double)(session.EndSOC ?? session.StartSOC ?? 0),
                        ProgressPercent = session.Status == "PoweredOff" || session.Status == "Completed" ? 100.0 : 0.0,
                        Status = session.Status,
                        Message = GetStatusMessage(session.Status)
                    };
                }

                // Tính SOC hiện tại dựa trên thời gian
                if (!session.StartTime.HasValue || !session.ExpectTime.HasValue)
                {
                    return new ChargingProgressResult
                    {
                        Success = false,
                        Message = "Invalid session time"
                    };
                }

                TimeSpan elapsed = DateTime.Now - session.StartTime.Value;
                double elapsedSeconds = elapsed.TotalSeconds;

                TimeSpan totalDuration = session.ExpectTime.Value - session.StartTime.Value;
                double totalSeconds = totalDuration.TotalSeconds;

                // Tính % hoàn thành
                double progressRatio = Math.Min(elapsedSeconds / totalSeconds, 1.0);

                // Tính SOC hiện tại
                double socRange = (double)(session.TargetSOC - session.StartSOC ?? 0);
                double currentSOC = (double)(session.StartSOC ?? 0) + (socRange * progressRatio);
                currentSOC = Math.Round(currentSOC, 2);

                // Kiểm tra đã đạt target chưa
                bool isCompleted = currentSOC >= (double)(session.TargetSOC ?? 80);

                if (isCompleted && session.PowerOffTime == null)
                {
                    // Sạc xong - tự động ngắt điện
                    session.PowerOffTime = DateTime.Now;
                    session.EndSOC = session.TargetSOC;
                    session.Status = "PoweredOff";
                    // Gọi hàm hoàn tất session từ chính service này
                    await CompleteChargingSession(session.SessionId);

                    // Tính năng lượng đã nạp
                    decimal energyDelivered = ((session.TargetSOC ?? 80) - (session.StartSOC ?? 0)) / 100 *
                                             (session.Vehicle.BatteryGrossKWh ?? 0);
                    session.MeterStopKWh = (session.MeterStartKWh ?? 0) + energyDelivered;

                    session.LastUpdated = DateTime.Now;
                    await _context.SaveChangesAsync();

                    _logger.LogInformation(
                        "Session {SessionId} completed charging automatically",
                        session.SessionId);

                    return new ChargingProgressResult
                    {
                        Success = true,
                        IsCompleted = true,
                        CurrentSOC = (double)(session.TargetSOC ?? 80),
                        ProgressPercent = 100.0,
                        Status = "PoweredOff",
                        Message = "Sạc hoàn tất! Vui lòng tháo cáp trong 10 phút"
                    };
                }

                // Cập nhật progress vào DB
                session.EndSOC = (decimal)currentSOC;
                session.LastUpdated = DateTime.Now;
                await _context.SaveChangesAsync();

                return new ChargingProgressResult
                {
                    Success = true,
                    IsCompleted = false,
                    CurrentSOC = currentSOC,
                    ProgressPercent = progressRatio * 100,
                    Status = "Charging",
                    Message = $"Đang sạc... {currentSOC:F1}%"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting charging progress for session {SessionId}", sessionId);
                return new ChargingProgressResult
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// Phê duyệt session "Requiring" - CHỈ Admin/Employee
        /// </summary>
        public async Task<ServiceResult> ApproveChargingSession(long sessionId)
        {
            try
            {
                var session = await _context.ChargingSession
                    .Include(s => s.Vehicle)
                    .Include(s => s.Connector)
                        .ThenInclude(c => c.Charger)
                    .FirstOrDefaultAsync(s => s.SessionId == sessionId);

                if (session == null || session.Status != "Requiring")
                {
                    return new ServiceResult
                    {
                        Success = false,
                        Message = "Session không hợp lệ hoặc đã được xử lý"
                    };
                }

                // Tính toán lại thời gian và công suất
                var vehicle = session.Vehicle;
                var connector = session.Connector;

                bool isChargerAC = connector.Charger.ChargerType?.Equals("AC", StringComparison.OrdinalIgnoreCase) == true;
                decimal vehicleMaxPower = isChargerAC
                    ? (vehicle.MaxAcChargeKW ?? 0)
                    : (vehicle.MaxDcChargeKW ?? 0);
                decimal chargerMaxPower = connector.Charger.MaxPowerKW ?? 0;
                decimal powerX = Math.Min(vehicleMaxPower, chargerMaxPower);

                decimal batteryGross = vehicle.BatteryGrossKWh ?? 0;
                decimal currentCapacity = ((session.StartSOC ?? 0) / 100) * batteryGross;
                session.MeterStartKWh = currentCapacity;

                decimal requiredCapacity = (((session.TargetSOC ?? 80) - (session.StartSOC ?? 0)) / 100) * batteryGross;
                decimal timeSeconds = (requiredCapacity / powerX) * 3600;

                // Cập nhật session
                session.Status = "Charging";
                session.StartTime = DateTime.Now;
                session.ExpectTime = DateTime.Now.AddSeconds((double)timeSeconds);
                session.LastUpdated = DateTime.Now;

                // Cập nhật connector
                connector.Status = "InUse";

                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Session {SessionId} approved and started charging",
                    session.SessionId);

                return new ServiceResult
                {
                    Success = true,
                    Message = "Đã phê duyệt và bắt đầu sạc"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving session {SessionId}", sessionId);
                return new ServiceResult
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// Từ chối session - CHỈ Admin/Employee
        /// </summary>
        public async Task<ServiceResult> RejectChargingSession(long sessionId)
        {
            try
            {
                var session = await _context.ChargingSession
                    .Include(s => s.Connector)
                    .FirstOrDefaultAsync(s => s.SessionId == sessionId);

                if (session == null)
                {
                    return new ServiceResult
                    {
                        Success = false,
                        Message = "Session not found"
                    };
                }

                session.Status = "Rejected";
                session.EndTime = DateTime.Now;
                session.LastUpdated = DateTime.Now;

                // Free connector
                if (session.Connector != null && session.Connector.Status == "InUse")
                {
                    session.Connector.Status = "Available";
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Session {SessionId} rejected", sessionId);

                return new ServiceResult
                {
                    Success = true,
                    Message = "Đã từ chối phiên sạc"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting session {SessionId}", sessionId);
                return new ServiceResult
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// Ngắt điện khẩn cấp - Admin/Employee hoặc Customer (của chính mình)
        /// </summary>
        public async Task<ServiceResult> StopChargingSession(long sessionId)
        {
            try
            {
                var session = await _context.ChargingSession
                    .Include(s => s.Vehicle)
                    .Include(s => s.Connector)
                    .FirstOrDefaultAsync(s => s.SessionId == sessionId);

                if (session == null)
                {
                    return new ServiceResult
                    {
                        Success = false,
                        Message = "Session not found"
                    };
                }

                session.PowerOffTime = DateTime.Now;
                session.EndSOC = session.EndSOC ?? session.StartSOC;
                session.Status = "Stopped";

                // Tính năng lượng đã nạp
                if (session.EndSOC.HasValue && session.StartSOC.HasValue && session.Vehicle != null)
                {
                    decimal energyDelivered = ((session.EndSOC.Value - session.StartSOC.Value) / 100) *
                                             (session.Vehicle.BatteryGrossKWh ?? 0);
                    session.MeterStopKWh = (session.MeterStartKWh ?? 0) + energyDelivered;
                }

                session.LastUpdated = DateTime.Now;

                // Free connector
                if (session.Connector != null)
                {
                    session.Connector.Status = "Available";
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Session {SessionId} stopped manually", sessionId);

                return new ServiceResult
                {
                    Success = true,
                    Message = "Đã ngắt điện"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error stopping session {SessionId}", sessionId);
                return new ServiceResult
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
     
        public async Task<ServiceResult> CompleteChargingSession(long sessionId)
        {
            try
            {
                var session = await _context.ChargingSession
                    .Include(s => s.Connector)
                    .Include(s => s.Vehicle)
                        .ThenInclude(v => v.Customer)
                    .FirstOrDefaultAsync(s => s.SessionId == sessionId);

                if (session == null)
                    return new ServiceResult { Success = false, Message = "Session not found" };

                // Chỉ hoàn tất nếu chưa hoàn tất
                if (session.Status != "Completed")
                {
                    session.EndTime = DateTime.Now;
                    session.Status = "Completed";
                    session.LastUpdated = DateTime.Now;

                    if (session.Connector != null)
                    {
                        session.Connector.Status = "Available";
                    }

                    await _context.SaveChangesAsync();
                }

                // ============================================
                // 1. Tạo invoice nếu chưa có
                // ============================================

                var invoice = await _context.Invoice
                    .FirstOrDefaultAsync(i => i.SessionId == sessionId);

                if (invoice == null)
                {
                    invoice = await _invoiceService.GenerateInvoice(sessionId);
                    _logger.LogInformation("Invoice generated for session {SessionId}", sessionId);
                }
                /**/

                // ============================================
                // 2. Gửi email
                // ============================================

                await _invoiceService.SendInvoiceEmail(invoice.InvoiceId);

                return new ServiceResult { Success = true, Message = "Session completed and invoice sent" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing session {SessionId}", sessionId);
                return new ServiceResult { Success = false, Message = ex.Message };
            }
        }



        // Helper method
        private string GetStatusMessage(string status)
        {
            return status switch
            {
                "Requiring" => "Đang chờ phê duyệt",
                "Charging" => "Đang sạc",
                "PoweredOff" => "Đã ngắt điện - Vui lòng tháo cáp",
                "Stopped" => "Đã dừng sạc",
                "Completed" => "Hoàn tất",
                "Rejected" => "Đã từ chối",
                _ => "Không xác định"
            };
        }
    }
}

