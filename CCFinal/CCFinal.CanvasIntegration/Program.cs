using CCFinal.CanvasIntegration;
using CCFinal.CanvasIntegration.Database;
using Microsoft.EntityFrameworkCore;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostBuilderContext, config) => {
        if (!hostBuilderContext.HostingEnvironment.IsDevelopment())
            config.AddEnvironmentVariables("KEY__");
    })
    .ConfigureServices((hostContext, services) => {
        services.AddHostedService<Worker>();
        services.AddDbContext<CanvasIntegrationDbContext>(options =>
            options.UseSqlServer(hostContext.Configuration.GetConnectionString(nameof(CanvasIntegrationDbContext))
                                 ?? throw new InvalidOperationException(
                                     $"Connection string '{nameof(CanvasIntegrationDbContext)}' is not found")));
        services.AddLogging();
        services.AddSingleton<HttpClient>(_ => {
            var client = new HttpClient();
            var uri = hostContext.Configuration.GetSection("BaseUris")["Canvas"] ?? string.Empty;
            client.BaseAddress = new Uri(uri);

            return client;
        });
    })
    .Build();

// Force DB Migrations
using (var scope = host.Services.CreateScope()) {
    var dbContext = scope.ServiceProvider.GetRequiredService<CanvasIntegrationDbContext>();
    try {
        await dbContext.Database.EnsureCreatedAsync();
    }
    catch (Exception e) {
        Console.WriteLine(e);
    }
}

host.Run();
