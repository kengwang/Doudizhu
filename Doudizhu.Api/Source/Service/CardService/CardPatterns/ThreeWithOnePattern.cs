using Doudizhu.Api.Models;

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
}