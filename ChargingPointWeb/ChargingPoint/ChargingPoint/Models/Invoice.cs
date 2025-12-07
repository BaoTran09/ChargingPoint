using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ChargingPoint.DB;
namespace ChargingPoint.Models
{
    public class Invoice
    {
        [Key]
        public long InvoiceId { get; set; }

        [ForeignKey("ChargingSession")]
        public long? SessionId { get; set; }

        // Customer snapshot at the time of invoice creation
        public string? Snashot_CustomerName { get; set; }
        public string? Snashot_CustomerPhone { get; set; }
        public string? Snashot_CustomerEmail { get; set; }

        // Invoice details
        public long InvoiceNumber { get; set; }
        public string InvoiceTemplate { get; set; } = "01GTKT";
        public string InvoiceSymbol { get; set; } = "01GTKT0/001";
        
        // Financial details
        public decimal? TotalAmountService { get; set; }
        public decimal? TotalAmountTax { get; set; }
        public decimal? TotalAmountDiscount { get; set; }
        public decimal? Total { get; set; }

        // Payment information
        public string? PaymentLink { get; set; }
        public string? QRCodeData { get; set; }
        public string Status { get; set; } = "Draft"; // Draft, Pending, Paid, Cancelled
        public DateTime? PaidAt { get; set; }
        public string? PaymentMethod { get; set; }
        
        // Document paths
        public string? PdfFilePath { get; set; }
        public bool EmailSent { get; set; } = false;
        public DateTime? EmailSentAt { get; set; }
        
        // Signatures and notes
        public string? Customer_Signature { get; set; }
        public string? SignatureFile { get; set; }
        public string? Notes { get; set; }
        
        // Timestamps
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ExpireDate { get; set; }

        // Relationships
       [ForeignKey("CustomerId")]
        public long? CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
        
        public virtual ChargingSession? ChargingSession { get; set; }
        
        // Navigation property for invoice details
        public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();
    }
}