using GameService.Base;
using GameService.DTOs;

namespace GameService.Repositories;

    public interface IGameRepository 
    {
        Task<BaseResponseModel> CreateGame(GameDTO gameDTO);
        Task<BaseResponseModel> UpdateGame(UpdateGameDTO gameDTO, Guid gameId);
        Task<BaseResponseModel> RemoveGame(Guid gameId);
        Task<BaseResponseModel> GetAllGames();
        Task<BaseResponseModel> GetGamesByCategory(Guid categoryId);
    }
