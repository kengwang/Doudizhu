using Doudizhu.Api.Models;
using Doudizhu.Api.Models.GameLogic;
using Doudizhu.Api.Service.CardService;

namespace Tests;

public class AutoCardMachineTest
{
    [Test]
    public async Task Test1()
    {
        var patterns = typeof(CardPattern).Assembly.GetTypes()
                                          .Where(t=>t.IsAssignableTo(typeof(CardPattern)) && !t.IsAbstract)
                                          .Select(t=>(CardPattern)Activator.CreateInstance(t)!)
                                          .ToList();
        
        // Arrange
        var autoCardMachine = new AutoCardMachineService(patterns);
        
        // Action
        // generate random card
        var game = new Game();
        
        var userCards = Game.GetAllCards().OrderBy(_ => Random.Shared.Next()).Take(20).OrderBy(t=>t.Number).ToList();
        userCards = Game.GetAllCards();
        PrintCards(userCards);
        Console.WriteLine("====================================");
        var res = await autoCardMachine.FindBestMatchCard(game, new()
        {
            Game = game,
            User = new()
            {
                Name = "Test",
            },
            Cards = userCards
        });
        Console.WriteLine();
        Console.WriteLine();
        PrintCards(res);
    }

    private static void PrintCards(List<Card> cards)
    {
        foreach (var card in cards)
        {
            Console.Write(card.Number);
            Console.Write(" ");
        }
        Console.WriteLine();
    }
}