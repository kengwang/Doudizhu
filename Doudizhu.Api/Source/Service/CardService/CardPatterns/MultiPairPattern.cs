using Doudizhu.Api.Models;

namespace Doudizhu.Api.Service.CardService.CardPatterns;

public class MultiPairPattern : CardPattern
{
    public override string Name => "连对";
    public override CardPatternType PatternType => CardPatternType.MultiPair;

    public override bool IsMatched(List<Card> card)
    {
        if (card.Count < 6 || card.Count % 2 != 0)
            return false;
        if (card.Any(t => t.Number is CardNumber.BigJoker or CardNumber.SmallJoker))
            return false;
        
        return card.CountBy(t => t.Number).All(t => t.Value == 2);
    }

    public override List<Card> Order(List<Card> card)
        => card.OrderByDescending(t => t.Number).ThenBy(t=>t.Color).ToList();

    public override bool CanCover(CardSentence current, CardSentence last)
    {
        if (current.PatternType != CardPatternType.MultiPair || last.PatternType != CardPatternType.MultiPair)
            return false;
        if (current.Cards.Count >= 6 && current.Cards.Count != last.Cards.Count)
            return false;
        
        return current.Cards.MinBy(t=>t.Number)!.Number > last.Cards.MinBy(t=>t.Number)!.Number;
    }
}