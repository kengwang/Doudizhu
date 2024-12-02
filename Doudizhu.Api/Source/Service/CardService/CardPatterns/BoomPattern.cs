using Doudizhu.Api.Models;

namespace Doudizhu.Api.Service.CardService.CardPatterns;

public class BoomPattern : CardPattern
{
    public override string Name => "炸弹";
    public override CardPatternType PatternType => CardPatternType.Bomb;

    public override bool IsMatched(List<Card> card)
    {
        if (card.Count != 4) return false;
        return card.All(c => c.Number == card[0].Number);
    }

    public override List<Card> Order(List<Card> card)
        => card;

    public override bool CanCover(CardSentence current, CardSentence last)
    {
        if (current.PatternType != CardPatternType.Bomb)
            return false;
        if (current.Cards.Count != 4)
            return false;
        if (last.PatternType is CardPatternType.Bomb)
            return current.Cards[0].Number > last.Cards[0].Number;

        if (last.PatternType is CardPatternType.JokerBomb)
            return false;

        return true;
    }
}