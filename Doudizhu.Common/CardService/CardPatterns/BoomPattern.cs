using Doudizhu.Api.Models.GameLogic;

namespace Doudizhu.Api.Service.CardService.CardPatterns;

public class BoomPattern : CardPattern
{
    public override string Name => "炸弹";
    public override CardPatternType PatternType => CardPatternType.Bomb;

    public override bool IsMatched(List<Card> card)
    {
        if (card.Count != 4) return false;
        if (card[0].Number is CardNumber.BigJoker or CardNumber.SmallJoker)
            return false;
        return card.All(c => c.Number == card[0].Number);
    }

    public override List<Card> Order(List<Card> card)
        => card;

    public override bool CanCover(CardSentence current, CardSentence last)
    {
        if (current.PatternType != CardPatternType.Bomb)
            return false;
        if (current.Cards.Count != 4)
            return false;
        if (last.PatternType is CardPatternType.Bomb)
            return current.Cards[0].Number > last.Cards[0].Number;

        if (last.PatternType is CardPatternType.JokerBomb)
            return false;

        return true;
    }

    public override async Task<List<(List<Card> baseCards, int count)>> GetBaseAndNeedle(List<Card> cards, CardSentence? lastSentence)
    {
        var baseCard = -1;
        if (lastSentence?.Cards is { Count: > 1 })
        {
            baseCard = (int?)lastSentence.Cards.FirstOrDefault()?.Number ?? -1;
        }
        var counts = cards.CountBy(t => t.Number).Where(t=>(int)t.Key > baseCard).Where(t=>t.Value == 4).Select(t=>t.Key).ToList();
        var avas = cards.Where(t => counts.Contains(t.Number)).GroupBy(t => t.Number).Select(t=>(baseCards: t.ToList(), count: 0)).ToList();
        return avas;
    }
}