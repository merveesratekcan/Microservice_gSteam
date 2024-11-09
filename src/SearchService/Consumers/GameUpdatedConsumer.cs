using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Consumers
{
    public class GameUpdatedConsumer : IConsumer<GameUpdated>
    {
        private readonly IMapper _mapper;
        public GameUpdatedConsumer(IMapper mapper)
        {
            mapper = _mapper;
        }
        public async Task Consume(ConsumeContext<GameUpdated> context)
        {
            Console.WriteLine("--->Game Updated Consuming" + context.Message.Id);

           var objDTO= _mapper.Map<GameItem>(context.Message);
           var result = await DB.Update<GameItem>()
                .Match(a => a.ID == context.Message.Id.ToString())
                .ModifyOnly(x=> new {
                    x.CategoryId,
                    x.RecommendedSystemRequirement,
                    x.MinimumSystemRequirement,
                    x.GameDescription,
                    x.Price,
                    x.GameAuthor,
                    x.GameName,
                }, objDTO).ExecuteAsync();
             
            if(!result.IsAcknowledged)
            {
                Console.WriteLine("Error in updating the game");
            }
        }
    }
}