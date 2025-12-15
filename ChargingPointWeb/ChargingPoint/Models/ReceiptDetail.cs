using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ChargingPoint.DB;

namespace ChargingPoint.Models
{
    public class ReceiptDetail
    {
        [Key, Column(Order = 0)]
        [ForeignKey("Receipt")]
        public long ReceiptId { get; set; }
        
        [Key, Column(Order = 1)]
        public int STT { get; set; } // Sequence number
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        [StringLength(20)]
        public string? DebitAccount { get; set; }
        
        [StringLength(20)]
        public string? CreditAccount { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        
        [ForeignKey("Invoice")]
        public long? RefInvoiceId { get; set; }
        
        // Navigation properties
        public virtual Receipt Receipt { get; set; }
        public virtual Invoice? RefInvoice { get; set; }
    }
}