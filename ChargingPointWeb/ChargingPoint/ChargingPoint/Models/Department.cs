using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ChargingPoint.DB;

namespace ChargingPoint.Models
{
    [Table("Department")]
    public class Department
    {
        [Key]
        [StringLength(200)]
        public string Id { get; set; }

        public string Name { get; set; }

        [StringLength(200)]
        public string Branch { get; set; }
    }

}