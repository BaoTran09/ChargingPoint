using ChargingPoint.DB;
namespace ChargingPoint.Models
{
    public class EmployeeViewModel
    {
       
            public long EmployeeId { get; set; }
            public string Email { get; set; }
            public string Username { get; set; }
            public string FullName { get; set; }
            public string JobTitle { get; set; }
            public string Status { get; set; } = "Active";
      
    }
}
