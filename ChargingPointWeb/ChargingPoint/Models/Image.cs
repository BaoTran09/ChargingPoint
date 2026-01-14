using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChargingPoint.Models
{


    public class Image
    {
        [Key]
        public long Id { get; set; }

        // Liên kết nghiệp vụ
        [Required]
        public string EntityType { get; set; }   // STATION, CHARGER, USER, INCIDENT

        [Required]
        public long EntityId { get; set; }

        // Cloudinary
        [Required]
        public string ImageUrl { get; set; }

        [Required]
        public string PublicId { get; set; }

        // Phục vụ hiển thị
        public string? Caption { get; set; }
        public int DisplayOrder { get; set; } = 0;
        public bool IsPrimary { get; set; } = false;

        // Quản lý
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }















    /* public class ImageCategory
     {
         [Key]
         public String Id { get; set; } // Ví dụ: "VEHICLE_VF8", "STATION_01","CHARGER_0123456"
         public string SubCategory { get; set; } // Ví dụ: "AVATAR" (USER), "MAIN" (STATION/CHARGER), 
         public string? Name { get; set; }
         public string? Path { get; set; }



         // Mối quan hệ Navigation
         public virtual ICollection<Image> Images { get; set; } = new List<Image>();
     }

     public class Image
     {
         [Key]
         [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
         public int Id { get; set; } // 
         public string FileName { get; set; }
         public string? Caption { get; set; }
         public int DisplayOrder { get; set; } = 0;
         public bool IsPrimary { get; set; } = false;
         public bool Isactive { get; set; } = false;
      public string  CategoryId { get; set; }
         public int Public_id { get; set; }
         [StringLength(50)]
         public string Imagetype { get; set; }
         public int file_size { get; set; }

         public DateTime UploadDate{ get; set; } = DateTime.UtcNow;

         [ForeignKey("CategoryId")]
         public virtual ImageCategory Category { get; set; }

         // Thuộc tính tính toán để lấy Link ảnh đầy đủ
         [NotMapped]
         public string FullUrl => $"{Category?.Path}{FileName}";
     }*/
}
