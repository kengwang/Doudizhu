using Doudizhu.Api.Models;
using Doudizhu.Api.Models.GameLogic;

namespace Doudizhu.Api.Service.CardService.CardPatterns;

public class JokerBoomPattern : CardPattern
{
    public override string Name => "王炸";
    public override CardPatternType PatternType => CardPatternType.JokerBomb;

    public override bool IsMatched(List<Card> card)
    {
        if (card.Count != 2)
            return false;

        return card.All(t => t.Number is CardNumber.SmallJoker or CardNumber.BigJoker);
    }

    public override List<Card> Order(List<Card> card)
        => card.OrderByDescending(t => t.Number).ToList();

    public override bool CanCover(CardSentence current, CardSentence last)
        => current.PatternType is CardPatternType.JokerBomb;

    public override async Task<List<(List<Card> baseCards, int count)>> GetBaseAndNeedle(List<Card> cards, CardSentence? lastCardSentence)
    {
        var counts = cards.Where(t=>t.Number is CardNumber.SmallJoker or CardNumber.BigJoker).ToList();
        if (counts.Count < 2)
            return new();
        return [(counts, 0)];
        
    }
}