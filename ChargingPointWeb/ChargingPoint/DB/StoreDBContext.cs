using Microsoft.EntityFrameworkCore;   

namespace ChargingPoint.DB
{
    public class StoreDBContext : DbContext
    {
        public StoreDBContext(DbContextOptions<StoreDBContext> options) 
            : base(options)
        {

        }

        public DbSet<Station> Station { get; set; }

    
    }
    

}
