using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using ChargingPoint.DB;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChargingPoint.Services
{
    /// <summary>
    /// Background service tự động tạo invoice cho sessions đã hoàn thành
    /// Chạy mỗi 30 giây
    /// </summary>
    public class InvoiceBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<InvoiceBackgroundService> _logger;

        public InvoiceBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<InvoiceBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("🚀 Invoice Background Service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessCompletedSessions();
                    await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken); // Check every 30s
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Error in Invoice Background Service");
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // Wait longer on error
                }
            }

            _logger.LogInformation("🛑 Invoice Background Service stopped");
        }
        /*

        private async Task ProcessCompletedSessions()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreDBContext>();
            var invoiceService = scope.ServiceProvider.GetRequiredService<IInvoiceService>();

            // Look for completed sessions without invoices
            var completedSessions = await context.ChargingSession
                .Where(s => s.Status == "Completed" &&
                           s.EndTime != null &&
                           !context.Invoices.Any(i => i.SessionId == s.SessionId))
                .OrderBy(s => s.EndTime)
                .Take(10)
                .ToListAsync();

            if (completedSessions.Any())
            {
                _logger.LogInformation(
                    "📋 Found {Count} completed sessions without invoices",
                    completedSessions.Count);

                foreach (var session in completedSessions)
                {
                    try
                    {
                        _logger.LogInformation("💰 Generating invoice for session {SessionId}...", session.SessionId);

                        var invoice = await invoiceService.GenerateInvoice(session.SessionId);

                        _logger.LogInformation(
                            "✅ Invoice {InvoiceId} created and email sent for session {SessionId}",
                            invoice.InvoiceId,
                            session.SessionId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex,
                            "❌ Failed to generate invoice for session {SessionId}",
                            session.SessionId);
                    }

                    // Small delay between each invoice
                    await Task.Delay(1000);
                }
            }
        }
        */
        // In InvoiceBackgroundService.cs: InvoiceBackgroundService, modify the ProcessCompletedSessions method
        private async Task ProcessCompletedSessions()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreDBContext>();
            var invoiceService = scope.ServiceProvider.GetRequiredService<IInvoiceService>();

            try
            {
                // Include Vehicle and Customer in the query and filter out sessions without valid customers
                var completedSessions = await context.ChargingSession
                    .Include(s => s.Vehicle)
                        .ThenInclude(v => v.Customer)
                    .Where(s => s.Status == "Completed" &&
                               s.EndTime != null &&
                               s.Vehicle != null &&           // Ensure Vehicle exists
                               s.Vehicle.Customer != null &&  // Ensure Customer exists
                               !context.Invoice.Any(i => i.SessionId == s.SessionId))
                    .OrderBy(s => s.EndTime)
                    .Take(10)
                    .ToListAsync();

                _logger.LogInformation("🔍 Found {Count} sessions to process", completedSessions.Count);

                foreach (var session in completedSessions)
                {
                    try
                    {
                        _logger.LogInformation("💳 Processing session {SessionId} for customer {CustomerId}",
                            session.SessionId,
                            session.Vehicle?.Customer?.CustomerId);

                        // Double-check customer exists before proceeding
                        if (session.Vehicle?.Customer == null)
                        {
                            _logger.LogWarning("⚠️ Session {SessionId} has no valid customer", session.SessionId);
                            continue;
                        }

                        var invoice = await invoiceService.GenerateInvoice(session.SessionId);

                        _logger.LogInformation(
                            "✅ Invoice {InvoiceId} created for session {SessionId}",
                            invoice.InvoiceId,
                            session.SessionId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "❌ Failed to process session {SessionId}", session.SessionId);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error in ProcessCompletedSessions");
                throw; // Re-throw to trigger the retry logic in ExecuteAsync
            }
        }



















































    }
}