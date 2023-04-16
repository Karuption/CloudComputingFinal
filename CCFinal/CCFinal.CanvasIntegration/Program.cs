using CCFinal.CanvasIntegration;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services => {
        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();
