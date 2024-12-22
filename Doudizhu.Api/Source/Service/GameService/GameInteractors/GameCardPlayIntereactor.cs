﻿using Doudizhu.Api.Interfaces;
using Doudizhu.Api.Interfaces.Markers;
using Doudizhu.Api.Models.GameLogic;
using Doudizhu.Api.Service.CardService;
using Doudizhu.Api.Service.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Doudizhu.Api.Service.GameService;

public class GameCardPlayIntereactor(IHubContext<GameHub, IClientNotificator> hubContext,
                                     AutoCardMachineService autoCardMachineService,
                                     CardSentenizer cardSentenizer)
    : IRegisterSelfScopedService, IRegisterScopedServiceFor<IGameInteractor>, IGameInteractor
{
    public async Task PlayCard(Game game, GameUser gameUser, CardSentence? cardSentence)
    {
        game.LastCardSentence = cardSentence;
        game.LastUser = gameUser;
        game.Records.Add(
            new()
            {
                Game = game,
                GameUser = gameUser,
                CardSentence = cardSentence
            });
        var cards = cardSentence?.Cards;
        if (cards is { Count: > 0 })
            gameUser.Cards = gameUser.Cards.Except(cards).ToList();
        await hubContext.Clients.Group(game.Id.ToString()).UserPlayCards(gameUser, cardSentence);
    }

    public int Index => 3;
    public CancellationTokenSource CurrentCancellationTokenSource { get; set; } = new();

    public async Task EnterInteraction(Game game)
    {
        game.Status = GameStatus.Running;
        var currentPlayerIndex = game.Users.FindIndex(t => t.Role == GameUserRole.Landlord);

        while (true)
        {
            game.CurrentUser = game.Users[currentPlayerIndex % game.Users.Count];

            try
            {
                await hubContext.Clients.User(game.CurrentUser.User.Id.ToString())
                                .RequirePlayCards(game, game.CurrentUser);
                await Task.Delay(TimeSpan.FromSeconds(30), CurrentCancellationTokenSource.Token);
            }
            catch (Exception e)
            {
                var cards = await autoCardMachineService.FindBestMatchCard(game, game.CurrentUser);
                var cardSentence = cards.Count == 0 ? null : cardSentenizer.Sentenize(cards);
                await PlayCard(game, game.CurrentUser, cardSentence);
            }
            if (game.CurrentUser.Cards.Count == 0)
            {
                break;
            }
        }
    }
}