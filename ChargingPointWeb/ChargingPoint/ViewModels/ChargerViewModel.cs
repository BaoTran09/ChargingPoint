using ChargingPoint.Models;

using ChargingPoint.DB;
using System.ComponentModel.DataAnnotations;

namespace ChargingPoint.ViewModels
{
    /// <summary>
    /// ViewModel hiển thị modal yêu cầu sạc
    /// </summary>
    public class RequestChargingViewModel
    {
        public long ConnectorId { get; set; }
        public Connector Connector { get; set; } = null!;

        // Danh sách xe THỰC TẾ của khách hàng
        public List<IndividualVehicle> IndividualVehicles { get; set; } = new();

        // Thông tin trạm & trụ sạc
        public string? StationName { get; set; }
        public string? ChargerName { get; set; }
        public string? ChargerType { get; set; }
        public decimal? ChargerMaxPower { get; set; }
    }

    /// <summary>
    /// ViewModel submit yêu cầu sạc (từ form)
    /// </summary>
    public class RequestChargingSubmitViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn cổng sạc")]
        public long ConnectorId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn xe")]
        [StringLength(500)]
        public string VIN { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập mức pin mong muốn")]
        [Range(1, 100, ErrorMessage = "Mức pin phải từ 1-100%")]
        public decimal TargetSOC { get; set; } = 80m;
    }

    /// <summary>
    /// ViewModel cho trang Create/Edit Charger (Employee)
    /// </summary>
    public class ChargerFormViewModel
    {
        public long ChargerId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn trạm")]
        public long StationId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên trụ sạc")]
        [StringLength(200)]
        public string Name { get; set; } = null!;

        [StringLength(100)]
        public string? SerialNumber { get; set; }

        [StringLength(100)]
        public string? Model { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn loại sạc")]
        public string ChargerType { get; set; } = "AC"; // AC hoặc DC

        [Required(ErrorMessage = "Vui lòng nhập công suất")]
        [Range(1, 1000, ErrorMessage = "Công suất từ 1-1000 kW")]
        public decimal MaxPowerKW { get; set; }

        [Range(1, 3, ErrorMessage = "Số pha từ 1-3")]
        public int? Phases { get; set; }

        public decimal? OutputVoltageMin { get; set; }
        public decimal? OutputVoltageMax { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số cổng")]
        [Range(1, 10, ErrorMessage = "Số cổng từ 1-10")]
        public int PortCount { get; set; } = 1;

        [StringLength(100)]
        public string? Design { get; set; }

        [StringLength(500)]
        public string? Protections { get; set; }

        [StringLength(50)]
        public string? FirmwareVersion { get; set; }

        public DateTime? InstalledAt { get; set; }

        public string Status { get; set; } = "Online";

        [Required(ErrorMessage = "Vui lòng chọn loại xe hỗ trợ")]
        public string UseFor { get; set; } = "Car"; // Car hoặc Motorbike

        [StringLength(1000)]
        public string? Note { get; set; }

        // Upload ảnh
        public string? PrimaryImageUrl { get; set; }
        public IFormFile? ImageFile { get; set; }

        // Danh sách trạm (để dropdown)
        public List<Station>? Stations { get; set; }

    }

    /// <summary>
    /// ViewModel hiển thị chi tiết Charger với ảnh
    /// </summary>
    public class ChargerDetailViewModel
    {
        public Charger Charger { get; set; } = null!;
        public List<Connector> Connectors { get; set; } = new();
        public List<Image> Images { get; set; } = new();
        public string? PrimaryImageUrl { get; set; }

        // Thống kê
        public int TotalSessions { get; set; }
        public int ActiveSessions { get; set; }
        public decimal TotalEnergyDelivered { get; set; }
    }
}