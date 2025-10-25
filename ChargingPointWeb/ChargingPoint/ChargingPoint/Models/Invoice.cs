using System.ComponentModel.DataAnnotations;

namespace ChargingPoint.Models
{
    public class Invoice
    {
        [Key]
        public string InvoiceId { get; set; } // Mã duy nhất hóa đơn
        public long SessionId { get; set; } // Liên kết với phiên sạc
        public DateTime CreatedAt { get; set; } // Ngày tạo
        public decimal? TotalEnergyKWh { get; set; } // Điện năng tiêu thụ
        public decimal UnitPrice { get; set; } // Giá đơn vị
        public decimal? ExtraFee { get; set; } // Phí thêm
        public decimal? IdleUnitPrice { get; set; } // Giá phí idle
        public int? OverDueMinutes { get; set; } // Phút trễ
        public decimal? Tax { get; set; } = 10.00m; // Thuế
        public string PaymentLink { get; set; } // Link thanh toán
        public int? EmployeeId { get; set; } // ID nhân viên
        public DateTime? ExpireDate { get; set; } // Ngày hết hạn
        public string Status { get; set; } = "unpaid"; // Trạng thái

        public ChargingSession ChargingSession { get; set; } // Liên kết với ChargingSession
    }
}
