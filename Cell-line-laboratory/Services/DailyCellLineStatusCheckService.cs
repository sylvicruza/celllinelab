using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Cell_line_laboratory.Data;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Cell_line_laboratory.Utils;

namespace Cell_line_laboratory.Services
{
    public class DailyCellLineStatusCheckService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<DailyCellLineStatusCheckService> _logger;

        private bool _emailSentForToday = false;

        public DailyCellLineStatusCheckService(IServiceProvider services, ILogger<DailyCellLineStatusCheckService> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("DailyCellLineStatusCheckService is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _services.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<Cell_line_laboratoryContext>(); // Replace with your DbContext class

                    var usedRowCount = await dbContext.CellLine.CountAsync(cl => cl.Status == "Used");

                    if (usedRowCount >= 1 && !_emailSentForToday)
                    {
                        var totalRowCount = await dbContext.CellLine.CountAsync();
                        var usedPercentage = (double)usedRowCount / totalRowCount * 100;

                        _logger.LogInformation($"Used Row Count: {usedRowCount}, Total Row Count: {totalRowCount}, Used Percentage: {usedPercentage}%");

                        if (usedPercentage >= 50)
                        {
                            // Send email notification to admin
                            await SendEmailNotificationAsync("AdminUserId", $"CellLine Usage threshold have reached: {usedPercentage}%");

                            _logger.LogInformation("Sending email notification to admin.");

                            _emailSentForToday = true; // Mark email as sent for today
                        }
                    }
                }

                // Check every minute
                await Task.Delay(60000, stoppingToken);
            }

            _logger.LogInformation("DailyCellLineStatusCheckService is stopping.");
        }

        private async Task SendEmailNotificationAsync(string userId, string message)
        {
            

            if (string.IsNullOrWhiteSpace(userId))
            {
                _logger.LogWarning($"Unable to send notification to user {userId}: no email address found.");
                return;
            }

            try
            {
      
                await EmailSender.SendEmailAsync(userId,"Notification", message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending notification to user.");
            }
        }
    }
}


