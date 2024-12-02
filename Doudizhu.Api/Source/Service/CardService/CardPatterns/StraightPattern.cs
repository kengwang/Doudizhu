using Doudizhu.Api.Models;

namespace Doudizhu.Api.Service.CardService.CardPatterns;

public class StraightPattern : CardPattern
{
    public override string Name => "顺子";
    public override CardPatternType PatternType => CardPatternType.Straight;

    public override bool IsMatched(List<Card> card)
    {
        if (card.Count < 5)
            return false;

        if (card.Any(t => t.Number is CardNumber.Two or CardNumber.SmallJoker or CardNumber.BigJoker))
            return false;
        var sortedCards = card.OrderBy(t=>t.Number).ToList();
        for (var i = 0; i < sortedCards.Count - 1; i++)
        {
            if (card[i + 1].Number - card[i].Number != 1)
                return false;
        }
        
        return true;
    }

    public override List<Card> Order(List<Card> card)
        => card.OrderByDescending(t=>t.Number).ToList();

    public override bool CanCover(CardSentence current, CardSentence last)
    {
        if (current.PatternType != CardPatternType.Straight || last.PatternType != CardPatternType.Straight)
            return false;
        if (current.Cards.Count != last.Cards.Count)
            return false;
        
        return current.Cards.MinBy(t=>t.Number)!.Number > last.Cards.MinBy(t=>t.Number)!.Number;
    }
}