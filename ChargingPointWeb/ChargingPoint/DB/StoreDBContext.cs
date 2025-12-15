using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ChargingPoint.Models; 

namespace ChargingPoint.DB
{
    // Kế thừa IdentityDbContext nếu dùng Identity, hoặc DbContext thường nếu tự quản lý user
    public class StoreDBContext : IdentityDbContext<Users>
    {
        public StoreDBContext(DbContextOptions<StoreDBContext> options) : base(options)
        {
        }

        #region DbSet Definitions (Khai báo bảng)

        // --- Core Business ---
        public DbSet<Station> Station { get; set; }
        public DbSet<Charger> Charger { get; set; }
        public DbSet<Connector> Connector { get; set; }
        public DbSet<ChargingSession> ChargingSession { get; set; }
        public DbSet<Customer> Customer { get; set; }
        public DbSet<Vehicle> Vehicle { get; set; }

        // --- HR & Admin ---
        public DbSet<Department> Department { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<Employee> Employee { get; set; }
        // public DbSet<Account> Accounts { get; set; } // Nếu có bảng Account riêng ngoài Identity

        // --- Finance & Accounting ---
        public DbSet<Invoice> Invoice { get; set; }
        public DbSet<InvoiceDetail> InvoiceDetail { get; set; }

        public DbSet<Receipt> Receipt { get; set; }
        public DbSet<ReceiptDetail> ReceiptDetail { get; set; }

        public DbSet<Transaction> Transaction { get; set; } // Sửa tên class cho khớp model

        public DbSet<RevenueItem> RevenueItem { get; set; }
        

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Cấu hình mối quan hệ 1-1 giữa User và Employee
            modelBuilder.Entity<Users>()
                .HasOne<Employee>(u => u.Employee)
                .WithOne(e => e.User)
                .HasForeignKey<Employee>(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình mối quan hệ 1-1 giữa User và Customer
            modelBuilder.Entity<Users>()
                .HasOne(u => u.Customer)
                .WithOne(c => c.User)
                .HasForeignKey<Customer>(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // === 1. TÊN BẢNG CHÍNH XÁC ===
            modelBuilder.Entity<Station>().ToTable("Station");
            modelBuilder.Entity<Charger>().ToTable("Charger");
            modelBuilder.Entity<Connector>().ToTable("Connector");
            modelBuilder.Entity<Customer>().ToTable("Customer");
            modelBuilder.Entity<Vehicle>().ToTable("Vehicle");
            modelBuilder.Entity<ChargingSession>().ToTable("ChargingSession");
            modelBuilder.Entity<Department>().ToTable("Department");
            modelBuilder.Entity<Role>().ToTable("Role");
            modelBuilder.Entity<Employee>().ToTable("Employee");
            modelBuilder.Entity<Invoice>().ToTable("Invoices");
            modelBuilder.Entity<InvoiceDetail>().ToTable("InvoiceDetail");
            modelBuilder.Entity<Receipt>().ToTable("Receipt");
            modelBuilder.Entity<ReceiptDetail>().ToTable("ReceiptDetail");
            modelBuilder.Entity<Transaction>().ToTable("Transactions");
            modelBuilder.Entity<RevenueItem>().ToTable("RevenueItem");

            // === 2. KHÓA CHÍNH PHỨC HỢP ===
            modelBuilder.Entity<InvoiceDetail>()
                .HasKey(id => new { id.InvoiceId, id.STT });

            modelBuilder.Entity<ReceiptDetail>()
                .HasKey(rd => new { rd.ReceiptId, rd.STT });

        
          
            // === 5. CÁC FK KHÁC (đúng rồi, giữ nguyên) ===
            modelBuilder.Entity<Charger>()
                .HasOne(c => c.Station)
                .WithMany(s => s.Chargers)
                .HasForeignKey(c => c.StationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Connector>()
                .HasOne(c => c.Charger)
                .WithMany(ch => ch.Connectors)
                .HasForeignKey(c => c.ChargerId)
                .OnDelete(DeleteBehavior.Cascade);


            /*
                        // Role -> Employee (1-to-many)
                builder.Entity<Employee>()
                    .HasOne(e => e.Role)
                    .WithMany(r => r.Employees)
                    .HasForeignKey(e => e.RoleId)
                    .OnDelete(DeleteBehavior.SetNull);

                // Station -> Charger (1-to-many)
                builder.Entity<Charger>()
                    .HasOne(c => c.Station)
                    .WithMany(s => s.Chargers)
                    .HasForeignKey(c => c.StationId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Charger -> Connector (1-to-many)
                builder.Entity<Connector>()
                    .HasOne(c => c.Charger)
                    .WithMany(ch => ch.Connectors)
                    .HasForeignKey(c => c.ChargerId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Customer -> Vehicle (1-to-many)
                builder.Entity<Vehicle>()
                    .HasOne(v => v.Customer)
                    .WithMany(c => c.Vehicles)
                    .HasForeignKey(v => v.CustomerId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Connector -> ChargingSession (1-to-many)
                builder.Entity<ChargingSession>()
                    .HasOne(cs => cs.Connector)
                    .WithMany(c => c.ChargingSessions)
                    .HasForeignKey(cs => cs.ConnectorId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Vehicle -> ChargingSession (1-to-many)
                builder.Entity<ChargingSession>()
                    .HasOne(cs => cs.Vehicle)
                    .WithMany(v => v.ChargingSessions)
                    .HasForeignKey(cs => cs.VehicleId)
                    .OnDelete(DeleteBehavior.SetNull);

                // ChargingSession -> Invoice (1-to-many)
                builder.Entity<Invoice>()
                    .HasOne(i => i.ChargingSession)
                    .WithMany(cs => cs.Invoices)
                    .HasForeignKey(i => i.SessionId)
                    .OnDelete(DeleteBehavior.SetNull);

                // Customer -> Invoice (1-to-many)
                builder.Entity<Invoice>()
                    .HasOne(i => i.Customer)
                    .WithMany(c => c.Invoices)
                    .HasForeignKey(i => i.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Invoice -> InvoiceDetail (1-to-many)
                builder.Entity<InvoiceDetail>()
                    .HasOne(id => id.Invoice)
                    .WithMany(i => i.InvoiceDetails)
                    .HasForeignKey(id => id.InvoiceId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Invoice -> Transaction (1-to-many)
                builder.Entity<Transaction>()
                    .HasOne(t => t.Invoice)
                    .WithMany(i => i.Transactions)
                    .HasForeignKey(t => t.InvoiceId)
                    .OnDelete(DeleteBehavior.SetNull);

                // Receipt Relationships
                builder.Entity<Receipt>()
                    .HasOne(r => r.Customer)
                    .WithMany(c => c.Receipts)
                    .HasForeignKey(r => r.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);

                builder.Entity<Receipt>()
                    .HasOne(r => r.Employee)
                    .WithMany(e => e.Receipts)
                    .HasForeignKey(r => r.EmployeeId)
                    .OnDelete(DeleteBehavior.SetNull);

                builder.Entity<Receipt>()
                    .HasOne(r => r.Invoice)
                    .WithMany(i => i.Receipts)
                    .HasForeignKey(r => r.InvoiceId)
                    .OnDelete(DeleteBehavior.SetNull);

                builder.Entity<Receipt>()
                    .HasOne(r => r.Transaction)
                    .WithMany(t => t.Receipts)
                    .HasForeignKey(r => r.TransactionId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Receipt -> ReceiptDetail (1-to-many)
                builder.Entity<ReceiptDetail>()
                    .HasOne(rd => rd.Receipt)
                    .WithMany(r => r.ReceiptDetails)
                    .HasForeignKey(rd => rd.ReceiptId)
                    .OnDelete(DeleteBehavior.Cascade);

             */


















        }






    }
}