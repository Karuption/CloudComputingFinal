using CCFinal.CanvasIntegration;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services => {
        services.AddHostedService<Worker>();
        services.AddLogging();
    })
    .Build();

host.Run();
