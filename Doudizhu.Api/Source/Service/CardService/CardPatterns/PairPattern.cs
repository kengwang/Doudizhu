using Doudizhu.Api.Models;

namespace Doudizhu.Api.Service.CardService.CardPatterns;

public class PairPattern : CardPattern
{
    public override string Name => "对子";
    public override CardPatternType PatternType => CardPatternType.Pair;

    public override bool IsMatched(List<Card> card)
    {
        if (card.Count != 2)
            return false;
        return card[0].Number == card[1].Number;
    }

    public override List<Card> Order(List<Card> card)
        => card;

    public override bool CanCover(CardSentence current, CardSentence last)
    {
        if (current.PatternType != CardPatternType.Pair || last.PatternType != CardPatternType.Pair)
            return false;
        if (current.Cards.Count != 2 && last.Cards.Count != 2)
            return false;

        return current.Cards[0].Number > last.Cards[0].Number;
    }
}