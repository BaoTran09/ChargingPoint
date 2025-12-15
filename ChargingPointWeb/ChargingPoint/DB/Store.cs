using ChargingPoint.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ChargingPoint.DB
{
    [Table("Station")]
    public class Station
    {
        [Key]
        public long StationId { get; set; }

        [StringLength(200)]
        public string? Tag { get; set; }

        // GIỮ NOT NULL như cũ để không phá code hiện tại
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = null!;

        [StringLength(50)]
        public string? StationType { get; set; }

        // === CÁC CỘT MỚI – để nullable trước ===
        [StringLength(500)]
        public string? Street { get; set; }

        [StringLength(200)]
        public string? Ward { get; set; }

        [StringLength(200)]
        public string? District { get; set; }

        [StringLength(200)]
        public string? City { get; set; }

        [StringLength(200)]
        public string? Province { get; set; }

        // GIỮ LẠI cột Address cũ để không phá app cũ
        [StringLength(500)]
        public string? Address { get; set; }

        [Column(TypeName = "decimal(10,6)")]
        public decimal? Latitude { get; set; }

        [Column(TypeName = "decimal(10,6)")]
        public decimal? Longitude { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        [StringLength(20)]
        public string? Phone_Number { get; set; }

        [Column(TypeName = "datetime2(3)")]
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? BuiltDate { get; set; }

        // Tính toán địa chỉ đầy đủ (dùng trong API, view)
        [NotMapped]
        public string FullAddress => string.Join(", ", new[]
        {
        Street, Ward, District, City, Province
    }.Where(s => !string.IsNullOrEmpty(s)));

        public virtual ICollection<Charger> Chargers { get; set; } = [];

        public Boolean IsActive { get; set; } = true;
    }
}