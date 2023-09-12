using Microsoft.EntityFrameworkCore;
using Cell_line_laboratory.Data; // Update with the correct namespace

namespace Cell_line_laboratory.Services
{
    public class DataCleanupWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DataCleanupWorker> _logger;


        public DataCleanupWorker(IServiceProvider serviceProvider, ILogger<DataCleanupWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("DataCleanUpService is starting.");
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();

                    var dbContext = scope.ServiceProvider.GetRequiredService<Cell_line_laboratoryContext>(); // Update with the correct DbContext

                    // Calculate the date 1 day ago for testing
                   // var oneDayAgo = DateTime.UtcNow.AddDays(-1);

                    // Calculate the date 1 day ago for testing
                    var threeMonthsAgo = DateTime.UtcNow.AddMonths(-3);

                    // Query and delete temporarily deleted CellLines older than 1 day
                    var cellLinesToDelete = await dbContext.CellLine
                        .Where(cellLine => cellLine.IsMarkedForDeletion && cellLine.DeletionTimestamp < threeMonthsAgo)
                        .ToListAsync();

                    dbContext.CellLine.RemoveRange(cellLinesToDelete);

                    // Query and delete temporarily deleted PlasmidCollections older than 3 months
                    var plasmidCollectionsToDelete = await dbContext.PlasmidCollection
                        .Where(plasmid => plasmid.IsMarkedForDeletion && plasmid.DeletionTimestamp < threeMonthsAgo)
                        .ToListAsync();

                    dbContext.PlasmidCollection.RemoveRange(plasmidCollectionsToDelete);


                    await dbContext.SaveChangesAsync();

                    // Log information about the cleanup
                    _logger.LogInformation($"Data cleanup executed at: {DateTime.UtcNow}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred during data cleanup.");
                }

                // Wait for the next iteration
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }
    }
}
