using Contracts;
using MassTransit;

namespace FilterService.Controller;

public class GameCreatedFilterConsumer : IConsumer<GameCreated>
{
    public Task Consume(ConsumeContext<GameCreated> context)
    {
        throw new NotImplementedException();
    }
}