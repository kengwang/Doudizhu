using Doudizhu.Api.Models;

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
}