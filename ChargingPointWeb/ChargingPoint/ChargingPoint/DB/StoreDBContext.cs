using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ChargingPoint.Models;

namespace ChargingPoint.DB
{
    
    public class StoreDBContext : IdentityDbContext<Users>
    {
      

        public StoreDBContext(DbContextOptions<StoreDBContext> options) : base(options)

            {

            }

        public DbSet<Station> Station { get; set; }
        // public DbSet<Users> Users { get; set; }
        public DbSet<Charger> Charger { get; set; } // Bảng Charger
        public DbSet<Connector> Connector { get; set; } // Bảng Connector
        public DbSet<Vehicle> Vehicle { get; set; } // Bảng Vehicle
        public DbSet<ChargingSession> ChargingSession { get; set; } // Bảng ChargingSession
                                                                    // public DbSet<Invoice> Invoice { get; set; } // Bảng Invoice
        public DbSet<Customer> Customer { get; set; }


    }



}
