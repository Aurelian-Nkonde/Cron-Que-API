using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace cron_que.hubs;

[Authorize]
public class NotificationHub: Hub
{
    public override async Task OnConnectedAsync()
    {
        string? userId = Context.UserIdentifier;
        await base.OnConnectedAsync();
    }
}

