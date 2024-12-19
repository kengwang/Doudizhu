using Doudizhu.Api.Interfaces;
using Doudizhu.Api.Interfaces.Markers;
using Doudizhu.Api.Models;
using Doudizhu.Api.Models.GameLogic;
using Doudizhu.Api.Service.Hubs;
using Doudizhu.Api.Service.Repositories;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Doudizhu.Api.Service.GameService;

public class DistributeCardInteractor(GameHub hub,
                                   IHubContext<GameHub, IClientNotificator> hubContext,
                                   DbContext dbContext) : IRegisterSelfService

{
    public async Task DistributeCardAsync(Game game)
    {
        var cards = Game.GetAllCards();
        cards = cards.OrderBy(_ => Random.Shared.Next()).ToList();

        for (var i = 0; i < 3; i++)
        {
            game.Users[i].Cards = cards.Skip(i * 17).Take(17).ToList();
            var connectionId = hub.GetConnectionIdByUserId(game.Users[i].User.Id);

            if (connectionId is null)
                continue;

            await hubContext.Clients.Client(connectionId).ReceiveCards(game.Users[i].Cards);
        }
        game.ReservedCards.AddRange(cards.Skip(51).ToList());
        await dbContext.SaveChangesAsync();
    }
}