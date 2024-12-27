using System.Security.Claims;
using Doudizhu.Api.Interfaces;
using Doudizhu.Api.Service.GameService;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Doudizhu.Api.Service.Hubs;

public class GameHub(GameContainer gameContainer) : Hub<IClientNotificator>
{
    private readonly Dictionary<Guid, string> _connectionIdToUserId = new();
    
    public string? GetConnectionIdByUserId(Guid userId)
        => _connectionIdToUserId.GetValueOrDefault(userId);

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.Claims.FirstOrDefault(t => t.Type == ClaimTypes.NameIdentifier)?.Value;
        var gameUsers = gameContainer.GetGames().SelectMany(t => t.Users);
        var user = gameUsers.FirstOrDefault(t => t.User.Id == Guid.Parse(userId!));
        if (userId is null)
        {
            Context.Abort();
            return;
        }

        if (user != null)
            await Groups.AddToGroupAsync(Context.ConnectionId, user.GameId.ToString());

        // add user connection id to dictionary
        _connectionIdToUserId[Guid.Parse(userId)] = Context.ConnectionId;
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.Claims.FirstOrDefault(t => t.Type == ClaimTypes.NameIdentifier)?.Value;
        if (userId is null)
        {
            return;
        }
        var gameUsers = gameContainer.GetGames().SelectMany(t => t.Users);
        var user = gameUsers.FirstOrDefault(t => t.User.Id == Guid.Parse(userId));
        if (user is null)
        {
            return;
        }
        var gameId = user.GameId.ToString();
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);
    }
}