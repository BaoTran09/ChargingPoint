using System.ComponentModel.DataAnnotations;
/// ViewModel cho submit request sạc

namespace ChargingPoint.ViewModels
{
    public class RequestChargingSubmitViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn connector")]
        public long ConnectorId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn xe")]
        public long VehicleId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mức pin hiện tại")]
        [Range(0, 100, ErrorMessage = "Mức pin phải từ 0-100%")]
        [Display(Name = "Pin hiện tại (%)")]
        public decimal StartSOC { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mức pin mong muốn")]
        [Range(0, 100, ErrorMessage = "Mức pin phải từ 0-100%")]
        [Display(Name = "Sạc đến (%)")]
        public decimal TargetSOC { get; set; } = 80;
    }
}
