using System.Net.Http.Headers;
using CCFinal.CanvasIntegration;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostBuilderContext, config) => {
        if (!hostBuilderContext.HostingEnvironment.IsDevelopment())
            config.AddEnvironmentVariables("KEY__");
    })
    .ConfigureServices((hostContext, services) => {
        services.AddHostedService<Worker>();
        services.AddLogging();
        services.AddSingleton<HttpClient>(_ => {
            var client = new HttpClient();
            var uri = hostContext.Configuration.GetSection("BaseUris")["Canvas"] ?? string.Empty;
            client.BaseAddress = new Uri(uri);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
                hostContext.Configuration.GetSection("ApiKeys")["Canvas"]);

            return client;
        });
    })
    .Build();

host.Run();
