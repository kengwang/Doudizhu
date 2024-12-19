using Doudizhu.Api.Models;
using Doudizhu.Api.Models.GameLogic;

namespace Doudizhu.Api.Service.CardService.CardPatterns;

public class SinglePattern : CardPattern
{
    public override string Name => "单牌";
    public override CardPatternType PatternType => CardPatternType.Single;

    public override bool IsMatched(List<Card> card)
        => card.Count == 1;

    public override List<Card> Order(List<Card> card)
        => card;

    public override bool CanCover(CardSentence current, CardSentence last)
    {
        if (current.PatternType != CardPatternType.Single || last.PatternType != CardPatternType.Single)
            return false;
        if (current.Cards.Count != 1 && last.Cards.Count != 1)
            return false;

        return current.Cards[0].Number > last.Cards[0].Number;
    }
    
    public override async Task<List<(List<Card> baseCards, int count)>> GetBaseAndNeedle(
        List<Card> cards,
        CardSentence? lastSentence)
    {
        var baseCard = (int?)lastSentence?.Cards.FirstOrDefault()?.Number ?? -1;
        var counts = cards.Where(t=>(int)t.Number > baseCard).GroupBy(t => t.Number).ToList();
        var avas = counts.Select(t => (baseCards: cards.Where(c => c.Number == t.Key).Take(1).ToList(), 0)).ToList();
        return avas;
    }
}