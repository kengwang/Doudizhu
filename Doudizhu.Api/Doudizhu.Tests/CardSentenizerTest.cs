using Doudizhu.Api.Models.GameLogic;
using Doudizhu.Api.Service.CardService.CardPatterns;

namespace Tests;

public class CardSentenizerTest
{

    [Test,
    Arguments(true, new[] { CardNumber.A, CardNumber.A, CardNumber.A, CardNumber.A }),
    Arguments(true, new[] { CardNumber.Three, CardNumber.Three, CardNumber.Three, CardNumber.Three }),
    Arguments(false, new[] { CardNumber.BigJoker, CardNumber.BigJoker, CardNumber.BigJoker, CardNumber.BigJoker }),
    Arguments(false, new[] { CardNumber.Three, CardNumber.Three, CardNumber.Three, CardNumber.Two }),
    ]
    public async Task BoomPattern(bool result, params CardNumber[] numbers)
    {
        // Arrange
        var pattern = new BoomPattern();
        
        // Action
        var actual = pattern.IsMatched(ConvertToCards(numbers));

        await Assert.That(actual).IsEqualTo(result);
    }
    
    [Test,
     Arguments(true, new[] { CardNumber.SmallJoker, CardNumber.BigJoker }),
     Arguments(false, new[] { CardNumber.A, CardNumber.A }),
     Arguments(false, new[] { CardNumber.BigJoker })
    ]
    public async Task JokerBoomPattern(bool result, params CardNumber[] numbers)
    {
        // Arrange
        var pattern = new JokerBoomPattern();

        // Action
        var actual = pattern.IsMatched(ConvertToCards(numbers));
        

        // Assert
        await Assert.That(actual).IsEqualTo(result);
    }

    private static List<Card> ConvertToCards(CardNumber[] numbers)
    {
        return numbers.Select(t => new Card(t, CardColor.Meihua)).ToList();
    }
}