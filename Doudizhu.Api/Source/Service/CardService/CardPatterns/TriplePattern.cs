using Doudizhu.Api.Models;
using Doudizhu.Api.Models.GameLogic;

namespace Doudizhu.Api.Service.CardService.CardPatterns;

public class TriplePattern : CardPattern
{
    public override string Name => "三张";
    public override CardPatternType PatternType => CardPatternType.Triple;

    public override bool IsMatched(List<Card> card)
    {
        if (card.Count < 3 || card.Count % 3 != 0)
            return false;

        var counts = card.CountBy(t => t.Number).ToList();
        
        if (counts.Any(t=>t.Value != 3))
            return false;
        
        return true;
    }

    public override List<Card> Order(List<Card> card)
        => card.OrderByDescending(t => t.Number).ThenBy(t=>t.Color).ToList();

    public override bool CanCover(CardSentence current, CardSentence last)
    {
        if (current.PatternType != CardPatternType.Triple || last.PatternType != CardPatternType.Triple)
            return false;
        if (current.Cards.Count < 3 && current.Cards.Count != last.Cards.Count)
            return false;

        var currentNumbers = current.Cards.CountBy(t => t.Number).Where(t=>t.Value == 3).Select(t=>t.Key).Order().ToList();
        var lastNumbers = last.Cards.CountBy(t => t.Number).Where(t=>t.Value == 3).Select(t=>t.Key).Order().ToList();
        
        if (currentNumbers.Count < 1 || lastNumbers.Count < 1 || currentNumbers.Count != lastNumbers.Count)
            return false;
        
        return currentNumbers[0] > lastNumbers[0];
    }

    public override async Task<List<(List<Card> baseCards, int count)>> GetBaseAndNeedle(List<Card> cards, CardSentence? lastSentence)
    {
        var baseCard = -1;
        if (lastSentence?.Cards is { Count: > 1 })
        {
            baseCard = (int?)lastSentence.Cards.FirstOrDefault()?.Number ?? -1;
        }
        var counts = cards.CountBy(t => t.Number).Where(t=>(int)t.Key > baseCard).Where(t=>t.Value >= 3).Select(t=>t.Key).ToList();
        var avas = cards.Where(t => counts.Contains(t.Number)).GroupBy(t => t.Number).Select(t=>(baseCards: t.Take(3).ToList(), count: 0)).ToList();
        return avas;
    }
}