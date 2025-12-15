using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChargingPoint.Models
{
    public class RevenueItem
    {
        [Key]
        public long ItemId { get; set; }
        public string? ItemName { get; set; }
        public string? Unit { get; set; }
        public string? ItemType { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
    }
}