using Doudizhu.Api.Models;

namespace Doudizhu.Api.Service.CardService.CardPatterns;

public class SinglePattern : CardPattern
{
    public override string Name => "单牌";
    public override CardPatternType PatternType => CardPatternType.Single;

    public override bool IsMatched(List<Card> card)
        => card.Count == 1;

    public override List<Card> Order(List<Card> card)
        => card;

    public override bool CanCover(CardSentence current, CardSentence last)
    {
        if (current.PatternType != CardPatternType.Single || last.PatternType != CardPatternType.Single)
            return false;
        if (current.Cards.Count != 1 && last.Cards.Count != 1)
            return false;

        return current.Cards[0].Number > last.Cards[0].Number;
    }
}