using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ChargingPoint.DB;

namespace ChargingPoint.Models
{
    public class Receipt
    {
        [Key]
        public long ReceiptId { get; set; }
        
        [Required]
        [StringLength(50)]
        public string ReceiptNumber { get; set; }
        
        public DateTime ReceiptDate { get; set; } = DateTime.UtcNow;
        
        [StringLength(200)]
        public string? PayerName { get; set; }
        
        [StringLength(500)]
        public string? PayerAddress { get; set; }
        
        [StringLength(1000)]
        public string? Description { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }
        
        [StringLength(100)]
        public string? PaymentMethod { get; set; }
        
        [StringLength(50)]
        public string Status { get; set; } = "Posted";
        
        [ForeignKey("Customer")]
        public long? CustomerId { get; set; }
        
        [ForeignKey("Employee")]
        public long? EmployeeId { get; set; }
        
        [ForeignKey("Invoice")]
        public long? InvoiceId { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        [StringLength(100)]
        public string? CreatedBy { get; set; }
        
        [StringLength(100)]
        public string? TransactionId { get; set; }
        
        // Navigation properties
        public virtual Customer? Customer { get; set; }
        public virtual Employee? Employee { get; set; }
        public virtual Invoice? Invoice { get; set; }
        public virtual ICollection<ReceiptDetail> ReceiptDetails { get; set; } = new List<ReceiptDetail>();
    }
}