using GameService.Base;
using GameService.DTOs;

namespace GameService.Repositories;

    public interface IGameRepository 
    {
        Task<BaseResponseModel> CreateGame(GameDTO gameDTO);
    }
