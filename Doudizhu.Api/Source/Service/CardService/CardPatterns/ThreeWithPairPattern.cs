using Doudizhu.Api.Models;

namespace Doudizhu.Api.Service.CardService.CardPatterns;

public class ThreeWithPairPattern : CardPattern
{
    public override string Name => "三带对";
    public override CardPatternType PatternType => CardPatternType.ThreeWithPair;
    public override bool IsMatched(List<Card> card)
    {
        if (card.Count != 5)
            return false;
        var groups = card.CountBy(x => x.Number).ToList();
        return groups.Count == 2 && groups.All(t => t.Value is 2 or 3);
    }

    public override List<Card> Order(List<Card> card)
    {
        var groups = card.CountBy(x => x.Number).ToList();
        var three = groups.First(t => t.Value == 3).Key;
        var pair = groups.First(t => t.Value == 2).Key;
        return card.Where(t => t.Number == three).Concat(card.Where(t => t.Number == pair)).ToList();
    }

    public override bool CanCover(CardSentence current, CardSentence last)
    {
        if (current.PatternType != CardPatternType.ThreeWithPair || last.PatternType != CardPatternType.ThreeWithPair)
            return false;
        if (current.Cards.Count != 5 && last.Cards.Count != 5)
            return false;

        var curGroups = current.Cards.CountBy(x => x.Number).ToList();
        var lastGroups = last.Cards.CountBy(x => x.Number).ToList();
        var curThree = curGroups.First(t => t.Value == 3).Key;
        var lastThree = lastGroups.First(t => t.Value == 3).Key;

        return curThree > lastThree;
    }
}