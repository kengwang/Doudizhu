using Doudizhu.Api.Models;
using Doudizhu.Api.Models.GameLogic;

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

    public override async Task<List<(List<Card> baseCards, int count)>> GetBaseAndNeedle(List<Card> cards, CardSentence? lastSentence)
    {
        var baseCard = -1;

        if (lastSentence?.Cards is { Count: > 1 })
        {
            baseCard =(int)lastSentence.Cards.CountBy(t => t.Number).Where(t => t.Value == 4).MinBy(t => t.Key).Key;
        }
        var counts = cards.CountBy(t => t.Number).Where(t => t.Value == 4).Where(t=>(int)t.Key > baseCard).Select(t => t.Key).ToList();
        var avas = cards.Where(t => counts.Contains(t.Number)).GroupBy(t => t.Number).Select(t => (baseCards: t.ToList(), count: 2)).ToList();
        return avas;
    }
}