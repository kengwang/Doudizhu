using System.Security.Claims;
using Doudizhu.Api.Interfaces;
using Doudizhu.Api.Service.Repositories;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Doudizhu.Api.Service.Hubs;

public class GameHub(ApplicationDbContext dbContext) : Hub<IClientNotificator>
{
    private readonly Dictionary<Guid, string> _connectionIdToUserId = new();
    
    public string? GetConnectionIdByUserId(Guid userId)
        => _connectionIdToUserId.GetValueOrDefault(userId);

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.Claims.FirstOrDefault(t => t.Type == ClaimTypes.NameIdentifier)?.Value;
        var user = dbContext.GameUsers.FirstOrDefault(t => t.User.Id == Guid.Parse(userId!));
        if (userId is null || user is null)
        {
            Context.Abort();
            return;
        }
        
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
        var user = dbContext.GameUsers.FirstOrDefault(t => t.User.Id == Guid.Parse(userId));
        if (user is null)
        {
            return;
        }
        var gameId = user.GameId.ToString();
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);
    }
}