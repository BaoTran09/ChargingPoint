using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ChargingPoint.DB;

namespace ChargingPoint.Models
{
    public class InvoiceDetail
    {
        [Key, Column(Order = 0)]
        [ForeignKey("Invoice")]
        public long InvoiceId { get; set; }
        
        [Key, Column(Order = 1)]
        public int STT { get; set; } // Sequence number

        [ForeignKey("RevenueItem")]
        public long? ItemId { get; set; }
        
        // Item details
        public int? Quantities { get; set; } = 0;
        public string? Unit { get; set; }
        public decimal? UnitPrice { get; set; } = 0;    
        public decimal? Amount { get; set; } = 0;

        public decimal? DiscountPercent { get; set; } = 0;
        public decimal? DiscountAmount { get; set; } = 0;
        public decimal? Tax { get; set; } = 10; // Default 10%
        public decimal? TaxAmount { get; set; } = 0;

        //public decimal TotalLine => Math.Round(AmountAfterDiscount * (1 + Tax / 100), 2); // số tiền sau khi chiết khấu và tính thuế

        // Navigation properties
        public virtual Invoice? Invoice { get; set; }
        public virtual RevenueItem? RevenueItem { get; set; }
    }
}