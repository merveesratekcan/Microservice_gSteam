using GameService.Base;
using GameService.Data;
using GameService.DTOs;

namespace GameService.Repositories;

public class GameRepository : IGameRepository
{
    public Task<BaseResponseModel> CreateGame(GameDTO gameDTO)
    {
        throw new NotImplementedException();
    }
}