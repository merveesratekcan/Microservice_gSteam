using DiscountService.Models;
using GameService;
using Grpc.Net.Client;

namespace DiscountService.Services;

public class GrpcGameClient
{
    private readonly ILogger<GrpcGameClient> _logger;
    private readonly IConfiguration _configuration;

    public GrpcGameClient(ILogger<GrpcGameClient> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public Game GetGame(string gameId,string userId)
    {
        _logger.LogWarning("GetGame method called");
        var channel = GrpcChannel.ForAddress(_configuration["GrpcGame"]);
        var client=new GrpcGame.GrpcGameClient(channel);
        var request=new GetGameRequest
        {
            Id = gameId,
            UserId = userId
        };
        try{
            var response = client.GetGame(request);
            Game game = new Game
            {
                GameName = response.Game.GameName,
                Price = Convert.ToDecimal(response.Game.Price),
                VideoUrl = response.Game.VideoUrl,
                GameDescription = response.Game.GameDescription,
                MinimumSystemRequirement= response.Game.MinimumsystemRequirement,
                RecommendedSystemRequirement = response.Game.RecommendedSystemRequirement,
                UserId = response.Game.UserId,
                CategoryId=Guid.Parse(response.Game.CategoryId),
            };
            Console.WriteLine(response);
            return game;
            
        }
        catch (System.Exception ex)
        {
            _logger.LogError("Error while calling GetGame method");
            throw ex;
        }
    }


}