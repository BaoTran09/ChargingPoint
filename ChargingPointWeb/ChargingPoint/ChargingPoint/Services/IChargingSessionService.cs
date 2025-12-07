// ============================================
// 3. IChargingSessionService.cs (Interface)
// ============================================
namespace ChargingPoint.Services
    {
        public interface IChargingSessionService
        {
            Task<ChargingSessionResult> CreateChargingSession(
                long connectorId,
                long vehicleId,
                decimal startSOC,
                decimal targetSOC);

            Task<ChargingProgressResult> GetChargingProgress(long sessionId);

            Task<ServiceResult> ApproveChargingSession(long sessionId);

            Task<ServiceResult> RejectChargingSession(long sessionId);

            Task<ServiceResult> StopChargingSession(long sessionId);

            Task<ServiceResult> CompleteChargingSession(long sessionId);
        }

        // Result DTOs
        public class ChargingSessionResult
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public long? SessionId { get; set; }
            public string SessionStatus { get; set; }
            public DateTime? ExpectTime { get; set; }
        }

        public class ChargingProgressResult
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public bool IsCompleted { get; set; }
            public double CurrentSOC { get; set; }
            public double ProgressPercent { get; set; }
            public string Status { get; set; }
        }

        public class ServiceResult
        {
            public bool Success { get; set; }
            public string Message { get; set; }
        }
    }
