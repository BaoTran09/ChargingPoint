using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ChargingPoint.DB
{
    
    public class StoreDBContext : IdentityDbContext<Users>
    {
      

        public StoreDBContext(DbContextOptions<StoreDBContext> options) : base(options)

            {

            }

        public DbSet<Station> Station { get; set; }
       // public DbSet<Users> Users { get; set; }

    }



}
