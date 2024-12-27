using Doudizhu.Api.Models;
using Doudizhu.Api.Models.GameLogic;

namespace Doudizhu.Api.Service.CardService.CardPatterns;

public class ThreeWithOnePattern : CardPattern
{
    public override string Name => "三带一";
    public override CardPatternType PatternType => CardPatternType.ThreeWithOne;
    public override bool IsMatched(List<Card> card)
    {
        if (card.Count != 4)
            return false;

        return card.CountBy(t => t.Number).Any(t=>t.Value == 3);
    }

    public override List<Card> Order(List<Card> card)
    {
        var counts = card.CountBy(t => t.Number).ToList();
        var three = counts.FirstOrDefault(t => t.Value == 3).Key;
        var one = counts.FirstOrDefault(t => t.Value == 1).Key;
        return card.Where(t => t.Number == three).Concat(card.Where(t => t.Number == one)).ToList();
    }

    public override bool CanCover(CardSentence current, CardSentence last)
    {
        if (current.PatternType != CardPatternType.ThreeWithOne || last.PatternType != CardPatternType.ThreeWithOne)
            return false;
        if (current.Cards.Count != 4 && last.Cards.Count != 4)
            return false;

        var curNum = current.Cards.CountBy(t => t.Number).FirstOrDefault(t => t.Value == 3).Key;
        var lastNum = last.Cards.CountBy(t => t.Number).FirstOrDefault(t => t.Value == 3).Key;
        
        return curNum > lastNum;
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
        var avas = cards.Where(t => counts.Contains(t.Number)).GroupBy(t => t.Number).Select(t=>(baseCards: t.Take(3).ToList(), count: 1)).ToList();
        return avas;
    }
}