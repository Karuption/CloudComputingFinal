using CCFinal.CanvasIntegration.Database;
using CCFinal.CanvasIntegration.Entities;
using DotNetCore.CAP;
using Microsoft.EntityFrameworkCore;

namespace CCFinal.CanvasIntegration.Subscriptions;

//Gets called by the CAP sidecar process
public interface IKeyUpdate {
    Task UpdateApiKey(IntegrationCanvas? model, CancellationToken cancellationToken);
}

public class KeyUpdate : ICapSubscribe, IKeyUpdate {
    private readonly CanvasIntegrationDbContext _dbContext;
    private readonly ILogger<KeyUpdate> _logger;
    private static readonly int defaultInitSearchDays = 3 * 7;

    public KeyUpdate(CanvasIntegrationDbContext dbContext, ILogger<KeyUpdate> logger) {
        _dbContext = dbContext;
        _logger = logger;
    }

    [CapSubscribe("Integration.Canvas")]
    public async Task UpdateApiKey(IntegrationCanvas? model, CancellationToken cancellationToken) {
        if (model is null) {
            _logger.LogDebug("Null model");
            return;
        }

        var result = await _dbContext.Information.FirstOrDefaultAsync(
            x => x.UserID == Guid.Parse(model!.UserId), cancellationToken);

        var basePath = new string(model.CanvasUrl).Trim();
        if (!basePath.EndsWith('/'))
            basePath = basePath + "/";
        if (!basePath.StartsWith("http"))
            basePath = "http://" + basePath;

        if (result is null) {
            await _dbContext.Information.AddAsync(new UserInformation {
                UserID = Guid.Parse(model.UserId),
                Token = model.Key,
                BaseUrl = basePath,
                LastRunDateTime = new DateTime(),
                LastCanvasUpdateDateTime =
                    DateTime.UtcNow.Subtract(
                        TimeSpan.FromDays(defaultInitSearchDays)) //only start with a search of default days history
            }, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                $"Processed Message and created for User: {model.UserId}");

            return;
        }


        result.Token = model.Key;
        result.BaseUrl = basePath;
        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation($"Processed Message for User: {model.UserId}");
    }
}

public record IntegrationCanvas(string UserId, string Key, string CanvasUrl);