using Doudizhu.Api.Models;

namespace Doudizhu.Api.Service.CardService;

public class CardSetencizer(IEnumerable<CardPattern> sequences)
{
    public CardSentence? Sequenize(List<Card> cards)
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