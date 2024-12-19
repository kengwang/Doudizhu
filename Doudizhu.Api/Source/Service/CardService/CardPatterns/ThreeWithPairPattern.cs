using Doudizhu.Api.Models;
using Doudizhu.Api.Models.GameLogic;

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

    public override async Task<List<(List<Card> baseCards, int count)>> GetBaseAndNeedle(
        List<Card> cards,
        CardSentence? lastSentence)
    {
        var baseCard = -1;
        if (lastSentence?.Cards is { Count: > 1 })
        {
            baseCard = (int?)lastSentence.Cards.FirstOrDefault()?.Number ?? -1;
        }
        var counts = cards.CountBy(t => t.Number).Where(t=>(int)t.Key > baseCard).Where(t=>t.Value >= 3).Select(t=>t.Key).ToList();
        var avas = cards.Where(t => counts.Contains(t.Number)).GroupBy(t => t.Number).Select(t=>(baseCards: t.Take(3).ToList(), count: -1)).ToList();
        return avas;
    }
}