using AutoMapper;
using Contracts;
using Elastic.Clients.Elasticsearch;
using FilterService.Models;
using MassTransit;

namespace FilterService.Controller;

public class GameCreatedFilterConsumer : IConsumer<GameCreated>
{
    private readonly  IMapper _mapper;
    private readonly ElasticsearchClient _elasticClient;
    private const string IndexName = "filter_game";

    public GameCreatedFilterConsumer(IMapper mapper, ElasticsearchClient elasticClient)
    {
        _mapper = mapper;
        _elasticClient = elasticClient;
    }

    public async Task Consume(ConsumeContext<GameCreated> context)
    {
        Console.WriteLine("Consuming Filter Service For Created Game--->"+context.Message.GameName);
        var objDTO = _mapper.Map<GameFilterItem>(context.Message);
        objDTO.GameId = context.Message.Id;

        var elasticsearch = await _elasticClient.IndexAsync(objDTO,x=>x.Index(IndexName));
        if (!elasticsearch.IsValidResponse)
        {
            Console.WriteLine("Consuming process is not valid");
        }
    }
}