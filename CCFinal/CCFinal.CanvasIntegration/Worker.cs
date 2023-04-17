using CCFinal.CanvasIntegration.Entities;
using CCFinal.CanvasIntegration.Services;
using DotNetCore.CAP;

namespace CCFinal.CanvasIntegration;

public class Worker : BackgroundService {
    private readonly ILogger<Worker> _logger;
    private readonly IServiceProvider _service;

    public Worker(ILogger<Worker> logger, IServiceProvider service) {
        _logger = logger;
        _service = service;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        _logger.LogInformation("Worker Started");

        using var scope = _service.CreateScope();
        List<Task> taskliList = new();

        var capBootstrap = scope.ServiceProvider.GetService<IBootstrapper>();
        if (capBootstrap is not null && !capBootstrap.IsStarted)
            await capBootstrap.BootstrapAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested) {
            var canvasService = scope.ServiceProvider.GetService<ICanvasService>();

            if (canvasService is null)
                _logger.LogInformation($"Cannot resolve {nameof(canvasService)}");

            //Get User metadata
            List<UserInformation> users = await canvasService.GetUsers();

            // Process All Users that need updates in Parallel
            if (users.Count > 0)
                foreach (var userInformation in users)
                    taskliList.Add(canvasService.ProcessUser(userInformation, stoppingToken));

            await Task.WhenAll(taskliList);
            taskliList.Clear();

            await Task.Delay(TimeSpan.FromMinutes(1));
        }


        if (stoppingToken.IsCancellationRequested)
            _logger.LogInformation("Cancellation requested");

        Console.ReadKey();
    }
}