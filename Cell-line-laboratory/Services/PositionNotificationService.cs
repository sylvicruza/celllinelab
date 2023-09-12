using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cell_line_laboratory.Data;
using Cell_line_laboratory.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Cell_line_laboratory.Services
{
    public class PositionNotificationService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PositionNotificationService> _logger;

        public PositionNotificationService(
            IServiceProvider serviceProvider,
            ILogger<PositionNotificationService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("PositionNotificationService is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<Cell_line_laboratoryContext>();

                    // Calculate the percentage of positions in use
                    int totalPositions = 100; // Assuming 100 positions in total
                    var positionsInUse = dbContext.CellLine
                        .Select(c => c.Position)
                        .AsEnumerable()
                        .SelectMany(positions => positions.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                        .Select(int.Parse)
                        .ToList();

                    double percentageInUse = (double)positionsInUse.Count / totalPositions * 100;
                    if (percentageInUse >= 3) // Adjust the threshold as needed
                    {
                        // Trigger your desired action here when the condition is met
                        // For example, you can write to log, send an email to admin, etc.
                        _logger.LogInformation("Percentage of positions in use is above threshold.");
                    }

                    // Wait for a specific interval before checking again
                    await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
                }

                _logger.LogInformation("PositionNotificationService is stopping.");
            }
        }
    }
}
