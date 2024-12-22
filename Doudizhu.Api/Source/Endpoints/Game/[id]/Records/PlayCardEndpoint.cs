using System.Security.Claims;
using Doudizhu.Api.Models.GameLogic;
using Doudizhu.Api.Service.CardService;
using Doudizhu.Api.Service.GameService;
using Doudizhu.Api.Service.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Doudizhu.Api.Endpoints.Game.Records;

public class PlayCardEndpoint(ApplicationDbContext dbContext,
                              GameCardPlayIntereactor playIntereactor,
                              CardSentenizer cardSentenizer,
                              IEnumerable<CardPattern> patterns)
    : Endpoint<PlayCardRequest, Results<Ok, BadRequest, NotFound>>
{
    private readonly Dictionary<CardPatternType, CardPattern> _patterns = patterns.ToDictionary(t => t.PatternType);

    public override void Configure()
    {
        Post("/game/{id:guid}/records");
    }

    public override async Task<Results<Ok, BadRequest, NotFound>> ExecuteAsync(PlayCardRequest req,
                                                                               CancellationToken ct)
    {
        var gameId = Route<Guid>("id");
        var game = dbContext.Games
                            .Include(game => game.Users)
                            .ThenInclude(gameUser => gameUser.User)
                            .Include(game => game.Records)
                            .ThenInclude(gameRecord => gameRecord.CardSentence)
                            .Include(game => game.Records)
                            .ThenInclude(gameRecord => gameRecord.GameUser)
                            .ThenInclude(gameUser => gameUser.User)
                            .FirstOrDefault(t => t.Id == gameId);

        if (game is null || game.Status != GameStatus.Running)
            return TypedResults.NotFound();

        var userId = User.Claims.FirstOrDefault(t => t.Type == ClaimTypes.NameIdentifier)?.Value;
        var gameUser = game.Users.First(t => t.User.Id.ToString() == userId);
        CardSentence? cardSentence = null;
        var lastSentence = game.Records.LastOrDefault(t => t.CardSentence is { Cards.Count: > 0 });

        if (req.Cards.Count > 0)
        {
            cardSentence = cardSentenizer.Sentenize(req.Cards);

            if (cardSentence is null)
                return TypedResults.BadRequest();
        }
        else
        {
            if (lastSentence?.CardSentence is null || lastSentence.GameUser.User.Id.ToString() == userId)
            {
                // 应该你出牌, 你不出
                return TypedResults.BadRequest();
            }
        }


        if (cardSentence is not null)
        {
            if (lastSentence?.CardSentence is not null && lastSentence.GameUser.User.Id.ToString() != userId)
            {
                var canCover = _patterns[cardSentence.PatternType].CanCover(cardSentence, lastSentence.CardSentence);

                if (!canCover)
                    return TypedResults.BadRequest();
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