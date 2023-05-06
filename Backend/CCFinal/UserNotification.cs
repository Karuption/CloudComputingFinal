using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace CCFinal;

public class UserNotification : Hub {
    private readonly ILogger<UserNotification> _logger;
    private readonly UserManager<IdentityUser> _userManager;

    public UserNotification(UserManager<IdentityUser> userManager, ILogger<UserNotification> logger) {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task Register() {
        if (Context.User?.Identity?.IsAuthenticated ?? false) {
            var user = await _userManager.FindByNameAsync(Context.User.Identity.Name!);
            await Groups.AddToGroupAsync(Context.ConnectionId, user!.Id);
            _logger.LogDebug($"User {Context.UserIdentifier} Connected");
        } else {
            await Groups.AddToGroupAsync(Context.ConnectionId, "");
        }
    }

    public async Task UnRegister() {
        if (Context.User?.Identity?.IsAuthenticated ?? false) {
            var user = await _userManager.FindByNameAsync(Context.User.Identity.Name!);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, user!.Id);
        } else {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "");
        }
    }
}

