using Doudizhu.Api.Models.GameLogic;
using Doudizhu.Api.Service.CardService;
using Doudizhu.Api.Service.CardService.CardPatterns;

namespace Tests;

public class PatternBaseAndNeedleTest
{
    [Test]
    [Arguments(new[] { CardNumber.Three, CardNumber.Four, CardNumber.Five, CardNumber.Six, CardNumber.Seven },
        new[] { CardNumber.Four, CardNumber.Five, CardNumber.Six, CardNumber.Seven, CardNumber.Eight, CardNumber.Nine, CardNumber.Nine, CardNumber.Ten })]
    public async Task  Straight_GetBaseAndNeedle(CardNumber[] cards,CardNumber[] current)
    {
        // Arrange
        var pattern = new StraightPattern();
        var cardSentenizer = new CardSetenizer([new StraightPattern()]);
        
        // action
        var actual = await pattern.GetBaseAndNeedle(ConvertToCards(current), cardSentenizer.Sentenize(ConvertToCards(cards)));
    }
    
    private static List<Card> ConvertToCards(CardNumber[] numbers)
    {
        return numbers.Select(t => new Card(t, CardColor.Meihua)).ToList();
    }
}