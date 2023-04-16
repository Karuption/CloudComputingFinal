namespace CCFinal.CanvasIntegration;

public class Worker : BackgroundService {
    private readonly ILogger<Worker> _logger;
    private readonly HttpClient _client;

    public Worker(ILogger<Worker> logger, HttpClient client) {
        _logger = logger;
        _client = client;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        _logger.LogInformation("Worker Started");

        var response = await _client.GetAsync("courses", stoppingToken);

        _logger.LogCritical($"Status code: {response.StatusCode.ToString()}");
        _logger.LogCritical($"Status code: {await response.Content.ReadAsStringAsync(stoppingToken)}");

        //while (!stoppingToken.IsCancellationRequested) {
        //    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
        //    await Task.Delay(1000, stoppingToken);
        //}

        if (stoppingToken.IsCancellationRequested)
            _logger.LogInformation("Cancellation requested");
    }
}
