using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ChargingPoint.DB;

namespace ChargingPoint.Models
{
    public class Transaction
    {
        [Key]
        [StringLength(100)]
        public string TransactionId { get; set; }
        
        public DateTime? TransactionDate { get; set; }
        
        public int? FromBankCode { get; set; }
        
        [StringLength(100)]
        public string? FromBankName { get; set; }
        
        [StringLength(100)]
        public string? FromAccountName { get; set; }
        
        public int? ToBankCode { get; set; }
        
        [StringLength(100)]
        public string? ToBankName { get; set; }
        
        [StringLength(100)]
        public string? ToAccountName { get; set; }
        
        public string? TransactionCode { get; set; }
        
        [StringLength(20)]
        public string? TransactionType { get; set; } // received, sent
        
        public decimal? Amount { get; set; }
        
        [StringLength(1000)]
        public string? Content { get; set; }
        
        [StringLength(500)]
        public string? Url { get; set; }
        
        [ForeignKey("Invoice")]
        public long? InvoiceId { get; set; }
        public long? InvoiceNumber { get; set; }
        // Navigation property
        public virtual Invoice? Invoice { get; set; }
    }
}