using Contracts;
using MassTransit;

namespace GameService.Consumers;

public class GameCreatedFauldConsumer : IConsumer<Fault<GameCreated>>
{

    public async Task Consume(ConsumeContext<Fault<GameCreated>> context)
    {
        Console.WriteLine("--> Consuming faulty creation");
        var exception= context.Message.Exceptions.First();
        if(exception.ExceptionType=="System.ArgumentException")
        {
            context.Message.Message.RecommendedSystemRequirement="No recommended system requirement";
            await context.Publish(context.Message.Message);
        }
        else
        {
            Console.WriteLine("Not an argument exception");
        }
       
        
    }
}


