namespace ChargingPoint.Services
{
    public interface  IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
        Task SendInvoiceEmailAsync(string toEmail, string customerName, string invoiceId, byte[] pdfAttachment);
    
    }
}
