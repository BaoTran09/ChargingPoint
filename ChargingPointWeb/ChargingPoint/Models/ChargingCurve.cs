using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ChargingPoint.Models
{
    public class ChargingCurve
    {
        [Key]
        public int Id { get; set; }

        public long VehicleId { get; set; }

        public int SocFrom { get; set; }    // %
        public int SocTo { get; set; }      // %

        public decimal MaxPowerKW { get; set; } // công suất tối đa trong khoảng SOC


        public bool IsDcFastCharge { get; set; } = true; // điều kiện: Vehicle.DcChargingSupport=true  

        [ForeignKey("VehicleId")]
        public Vehicle Vehicles { get; set; }
    }
}
