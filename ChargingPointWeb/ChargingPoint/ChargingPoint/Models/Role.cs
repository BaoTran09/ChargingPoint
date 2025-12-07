using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChargingPoint.Models
{
    [Table("Role")]
    public class Role
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        [StringLength(100)]
        public string Level { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? SalaryBase { get; set; }

        // Navigation properties
        public virtual ICollection<Employee> Employee{ get; set; }
    }
}