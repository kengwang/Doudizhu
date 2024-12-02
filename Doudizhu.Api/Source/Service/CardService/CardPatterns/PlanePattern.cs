using Doudizhu.Api.Models;

namespace Doudizhu.Api.Service.CardService.CardPatterns;

public class PlanePattern : CardPattern
{
    public override string Name => "飞机";
    public override CardPatternType PatternType => CardPatternType.Plane;

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
        if (current.PatternType != CardPatternType.Plane || last.PatternType != CardPatternType.Plane)
            return false;
        if (current.Cards.Count < 3 && current.Cards.Count != last.Cards.Count)
            return false;

        var currentNumbers = current.Cards.CountBy(t => t.Number).Where(t=>t.Value == 3).Select(t=>t.Key).Order().ToList();
        var lastNumbers = last.Cards.CountBy(t => t.Number).Where(t=>t.Value == 3).Select(t=>t.Key).Order().ToList();
        
        if (currentNumbers.Count < 1 || lastNumbers.Count < 1 || currentNumbers.Count != lastNumbers.Count)
            return false;
        
        return currentNumbers[0] > lastNumbers[0];
    }
}