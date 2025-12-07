using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ChargingPoint.DB;

namespace ChargingPoint.Models
{
    [Table("Department")]
    public class Department
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        [StringLength(200)]
        public string Branch { get; set; }
    }

}