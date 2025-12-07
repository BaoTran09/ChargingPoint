using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ChargingPoint.Models;

namespace ChargingPoint.DB
{    [Table("Station")]
    public class Station
    {
        [Key]
        public long StationId { get; set; }
        
        [StringLength(200)]
        public string? Tag { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Name { get; set; }
        
        [StringLength(50)]
        public string? StationType { get; set; }
        
        [StringLength(500)]
        public string? Address { get; set; }
        
        [StringLength(20)]
        public string? Phone_Number { get; set; }
        
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        
        [StringLength(500)]
        public string? Notes { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? BuiltDate { get; set; }
        
        // Navigation properties
        public virtual ICollection<Charger> Charger { get; set; } = new List<Charger>();
    }
}