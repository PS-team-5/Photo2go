using Microsoft.AspNetCore.SignalR;

namespace Photo2GoAPI.Hubs;

public class NotificationHub : Hub
{
    public static string UserGroup(int userId) => $"user:{userId}";

    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var userIdRaw = httpContext?.Request.Query["userId"].ToString();

        if (int.TryParse(userIdRaw, out var userId) && userId > 0)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, UserGroup(userId));
        }

        await base.OnConnectedAsync();
    }
}

