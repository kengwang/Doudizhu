using Doudizhu.Api.Interfaces.Markers;
using Doudizhu.Api.Models;
using Doudizhu.Api.Models.GameLogic;

namespace Doudizhu.Api.Service.CardService;

public class CardSetenizer(IEnumerable<CardPattern> sequences) : IRegisterSelfService
{
    public CardSentence? Sentenize(List<Card> cards)
    {
        foreach (var cardPattern in sequences)
        {
            if (!cardPattern.IsMatched(cards))
                continue;

            return new()
            {
                PatternType = cardPattern.PatternType,
                Cards = cardPattern.Order(cards)
            };
        }

        return null;
    }
}