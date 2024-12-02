using Doudizhu.Api.Models;

namespace Doudizhu.Api.Service.CardService.CardPatterns;

public class FourWithPairPattern : CardPattern
{
    public override string Name => "四带二";
    public override CardPatternType PatternType => CardPatternType.FourWithPair;

    public override bool IsMatched(List<Card> card)
    {
        if (card.Count != 6)
            return false;
        var counts = card.CountBy(t=>t.Number).ToList();
        return counts.Any(t => t.Value is 4);
    }

    public override List<Card> Order(List<Card> card)
    {
        var counts = card.CountBy(t => t.Number).ToList();
        var four = counts.FirstOrDefault(t => t.Value == 4).Key;
        var res = new List<Card>();
        res.AddRange(card.Where(t => t.Number == four));
        res.AddRange(card.Where(t => t.Number != four).OrderByDescending(t=>t.Number));

        return res;
    }

    public override bool CanCover(CardSentence current, CardSentence last)
    {
        if (current.PatternType != CardPatternType.FourWithPair || last.PatternType != CardPatternType.FourWithPair)
            return false;
        if (current.Cards.Count != 6 && last.Cards.Count != 6)
            return false;

        var curCounts = current.Cards.CountBy(t => t.Number).FirstOrDefault(t => t.Value == 4);
        var lastCounts = last.Cards.CountBy(t => t.Number).FirstOrDefault(t => t.Value == 4);
        return curCounts.Key > lastCounts.Key;
    }
}