using System.ComponentModel.DataAnnotations;

namespace ChargingPoint.DB
{
    public class Station
    {
        [Key]  // 🚀 đánh dấu StationId là khóa chính
        public long StationId { get; set; }

        public string Tag { get; set; }      
        public string Name { get; set; }      
        public string StationType { get; set; } 
        public string Address { get; set; } 

        public decimal? Latitude { get; set; }   
        public decimal? Longitude { get; set; }  

        public string Notes { get; set; }   

        public DateTime CreatedAt { get; set; } 
        public DateTime? BuiltDate { get; set; }


    }








}
