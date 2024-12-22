using Doudizhu.Api.Interfaces;
using Doudizhu.Api.Interfaces.Markers;
using Doudizhu.Api.Models;
using Doudizhu.Api.Models.GameLogic;
using Doudizhu.Api.Service.Hubs;
using Doudizhu.Api.Service.Repositories;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Doudizhu.Api.Service.GameService;

public class DistributeCardIntereactor(GameHub hub,
                                       IHubContext<GameHub, IClientNotificator> hubContext)
    : IRegisterSelfScopedService, IRegisterScopedServiceFor<IGameInteractor>, IGameInteractor

{
    public int Index => 0;

    public CancellationTokenSource CurrentCancellationTokenSource { get; set; } = new();

    public async Task EnterInteraction(Game game)
    {
        game.Status = GameStatus.Starting;
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
    }
}