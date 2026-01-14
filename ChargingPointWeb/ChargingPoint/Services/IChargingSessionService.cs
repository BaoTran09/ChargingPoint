using System;
using System.Threading.Tasks;

namespace ChargingPoint.Services
{
    public interface IChargingSessionService
    {
        // Xóa hậu tố Async để khớp với cách đặt tên phổ biến hoặc giữ nguyên tùy bạn, 
        // nhưng phải khớp với file Service bên dưới.
        Task<ChargingSessionResult> CreateChargingSession(long connectorId, string vin, decimal targetSOC, long customerId);
        Task<ChargingProgressResult> GetChargingProgress(long sessionId);
        Task<ServiceResult> ApproveChargingSession(long sessionId);
        Task<ServiceResult> RejectChargingSession(long sessionId);
        Task<ServiceResult> StopChargingSession(long sessionId);
        Task<ServiceResult> CompleteChargingSession(long sessionId);
    }

    #region Result DTOs

    // Dùng kế thừa ServiceResult để code gọn sạch hơn (DRY)
    public class ChargingSessionResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public long SessionId { get; set; }
        public DateTime? ExpectTime { get; set; }
        public string? SessionStatus { get; set; }
        public int EstimatedDurationMinutes { get; set; }
    }

    public class ChargingProgressResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public double CurrentSOC { get; set; }
        public double ProgressPercent { get; set; }
        public string? Status { get; set; }
        public int ElapsedSeconds { get; set; }
        public int RemainingSeconds { get; set; }
        public double EnergyDeliveredKWh { get; set; }
    }

    public class ServiceResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
    }

    #endregion
}