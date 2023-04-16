namespace CCFinal.CanvasIntegration;

public class Worker : BackgroundService {
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger) {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        _logger.LogInformation("Worker Started");

        while (!stoppingToken.IsCancellationRequested) {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(1000, stoppingToken);
        }

        if (stoppingToken.IsCancellationRequested)
            _logger.LogInformation("Cancellation requested");
    }
}
