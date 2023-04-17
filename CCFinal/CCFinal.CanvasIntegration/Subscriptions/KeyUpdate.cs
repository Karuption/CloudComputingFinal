using CCFinal.CanvasIntegration.Database;
using CCFinal.CanvasIntegration.Entities;
using DotNetCore.CAP;
using Microsoft.EntityFrameworkCore;

namespace CCFinal.CanvasIntegration.Subscriptions;

//Gets called by the CAP sidecar process
public interface IKeyUpdate {
    Task UpdateApiKey(KeyUpdateModel? model, CancellationToken cancellationToken);
}

public class KeyUpdate : ICapSubscribe, IKeyUpdate {
    private readonly CanvasIntegrationDbContext _dbContext;
    private readonly ILogger<KeyUpdate> _logger;

    public KeyUpdate(CanvasIntegrationDbContext dbContext, ILogger<KeyUpdate> logger) {
        _dbContext = dbContext;
        _logger = logger;
    }

    [CapSubscribe("Integration.Canvas")]
    public async Task UpdateApiKey(KeyUpdateModel model, CancellationToken cancellationToken) {
        if (model is null) {
            _logger.LogDebug("Null model");
            return;
        }

        var result = await _dbContext.Information.FirstOrDefaultAsync(
            x => x.UserID == Guid.Parse(model!.UserId), cancellationToken);

        if (result is null) {
            await _dbContext.Information.AddAsync(new UserInformation {
                UserID = Guid.Parse(model.UserId),
                Token = model.Key,
                LastRunDateTime = new DateTime(),
                LastCanvasUpdateDateTime = new DateTime()
            }, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                $"Processed Message and created for User: {model.UserId} SentTimeStamp: {model.Timestamp}");

            return;
        }


        result.Token = model.Key;
        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation($"Processed Message for User: {model.UserId} SentTimeStamp: {model.Timestamp}");
    }
}

public record KeyUpdateModel(string Key, string UserId, DateTime Timestamp) { }