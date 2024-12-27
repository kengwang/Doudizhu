using System.Security.Claims;
using Doudizhu.Api.Models.GameLogic;
using Doudizhu.Api.Service.CardService;
using Doudizhu.Api.Service.GameService;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Doudizhu.Api.Endpoints.Game.Records;

public class PlayCardEndpoint(GameCardPlayIntereactor playIntereactor,
                              GameContainer gameContainer,
                              CardSentenizer cardSentenizer,
                              IEnumerable<CardPattern> patterns)
    : Endpoint<PlayCardRequest, Results<Ok, BadRequest, NotFound>>
{
    private readonly Dictionary<CardPatternType, CardPattern> _patterns = patterns.ToDictionary(t => t.PatternType);

    public override void Configure()
    {
        Post("/games/{id:guid}/records");
    }

    public override async Task<Results<Ok, BadRequest, NotFound>> ExecuteAsync(PlayCardRequest req,
                                                                               CancellationToken ct)
    {
        var gameId = Route<Guid>("id");
        var game = gameContainer.GetGame(gameId);
        if (game is null)
            return TypedResults.NotFound();
        game.Status = GameStatus.Running;
        var userId = User.Claims.FirstOrDefault(t => t.Type == ClaimTypes.NameIdentifier)?.Value;
        var gameUser = game.Users.First(t => t.User.Id.ToString() == userId);
        CardSentence? cardSentence = null;
        var lastSentence = game.Records.LastOrDefault(t => t.CardSentence is { Cards.Count: > 0 });

        if (req.Cards.Count > 0)
        {
            cardSentence = cardSentenizer.Sentenize(req.Cards);

            if (cardSentence is null)
            {
                Console.WriteLine("不能打");
                return TypedResults.BadRequest();
            }

        }
        else
        {
            if (lastSentence?.CardSentence is null || lastSentence.GameUser.User.Id.ToString() == userId)
            {
                // 应该你出牌, 你不出
                Console.WriteLine("你没出");
                return TypedResults.BadRequest();
            }
        }
        

        if (cardSentence is not null)
        {
            if (lastSentence?.CardSentence is not null && lastSentence.GameUser.User.Id.ToString() != userId)
            {
                var canCover = _patterns[cardSentence.PatternType].CanCover(cardSentence, lastSentence.CardSentence);

                if (!canCover)
                {
                    Console.WriteLine("打不过");
                    return TypedResults.BadRequest();

                }
            }
        }
        
        await playIntereactor.PlayCard(
            game,
            gameUser,
            cardSentence);
        return TypedResults.Ok();
    }
}

public class PlayCardRequest
{
    public List<Card> Cards { get; set; }
}