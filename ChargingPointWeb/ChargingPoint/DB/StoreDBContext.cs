using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ChargingPoint.Models;

using ChargingPoint.DB;

namespace ChargingPoint.DB
{
    public class StoreDBContext : IdentityDbContext<Users>
    {
        public StoreDBContext(DbContextOptions<StoreDBContext> options) : base(options)
        {
        }

        #region DbSet Definitions
        public DbSet<Station> Stations { get; set; }
        public DbSet<Charger> Chargers { get; set; }
        public DbSet<Connector> Connectors { get; set; }
        public DbSet<ChargingSession> ChargingSessions { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<IndividualVehicle> IndividualVehicles { get; set; }
        public DbSet<ChargingCurve> ChargingCurves { get; set; }
        public DbSet<Department> Departments { get; set; }
      //  public DbSet<Role> Roles { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceDetail> InvoiceDetails { get; set; }
        public DbSet<Receipt> Receipts { get; set; }
        public DbSet<ReceiptDetail> ReceiptDetails { get; set; }
        public DbSet<RevenueItem> RevenueItems { get; set; }
        public DbSet<Image> Images { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // === 1. ĐẶT TÊN BẢNG (Theo đúng Attribute [Table] trong code của bạn) ===
            modelBuilder.Entity<Station>().ToTable("Station");
            modelBuilder.Entity<Department>().ToTable("Department");
            modelBuilder.Entity<Employee>().ToTable("Employee");
            modelBuilder.Entity<Customer>().ToTable("Customer");
            modelBuilder.Entity<Role>().ToTable("Role");
            modelBuilder.Entity<Vehicle>().ToTable("Vehicle");
            modelBuilder.Entity<IndividualVehicle>().ToTable("IndividualVehicle");

            // Các bảng khác không có [Table]
            modelBuilder.Entity<Charger>().ToTable("Charger");
            modelBuilder.Entity<Connector>().ToTable("Connector");
            modelBuilder.Entity<ChargingCurve>().ToTable("ChargingCurve");
            modelBuilder.Entity<Image>().ToTable("Images");
            modelBuilder.Entity<RevenueItem>().ToTable("RevenueItem");
            modelBuilder.Entity<Invoice>().ToTable("Invoice");
            modelBuilder.Entity<InvoiceDetail>().ToTable("InvoiceDetail");
            modelBuilder.Entity<Receipt>().ToTable("Receipt");
            modelBuilder.Entity<ReceiptDetail>().ToTable("ReceiptDetail");

            // === 2. KHÓA CHÍNH PHỨC HỢP (Composite Keys) ===
            // Cấu hình STT là một phần của khóa chính cho chi tiết hóa đơn và phiếu thu
            modelBuilder.Entity<InvoiceDetail>()
                .HasKey(id => new { id.InvoiceId, id.STT });

            modelBuilder.Entity<ReceiptDetail>()
                .HasKey(rd => new { rd.ReceiptId, rd.STT });

            // === 3. MỐI QUAN HỆ IDENTITY (1-1) ===
            // Kết nối tài khoản User với Employee và Customer thông qua UserId
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.User)
                .WithOne(u => u.Employee)
                .HasForeignKey<Employee>(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Customer>()
                .HasOne(c => c.User)
                .WithOne(u => u.Customer)
                .HasForeignKey<Customer>(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // === 4. CÁC MỐI QUAN HỆ NGHIỆP VỤ ===

            // Một trạm sạc có nhiều bộ sạc
            modelBuilder.Entity<Charger>()
                .HasOne(c => c.Station)
                .WithMany(s => s.Chargers)
                .HasForeignKey(c => c.StationId);

            // Một bộ sạc có nhiều đầu nối (Connector)
            modelBuilder.Entity<Connector>()
                .HasOne(c => c.Charger)
                .WithMany(ch => ch.Connectors)
                .HasForeignKey(c => c.ChargerId);

            // Một mẫu xe (Vehicle) có nhiều xe thực tế (IndividualVehicle)
            modelBuilder.Entity<IndividualVehicle>()
                .HasOne(iv => iv.Vehicle)
                .WithMany(v => v.IndividualVehicles)
                .HasForeignKey(iv => iv.VehicleId);

            // Một mẫu xe có nhiều mốc trong đường cong sạc
            modelBuilder.Entity<ChargingCurve>()
                .HasOne(cc => cc.Vehicles)
                .WithMany()
                .HasForeignKey(cc => cc.VehicleId);
        }
    }
}