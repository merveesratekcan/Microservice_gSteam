using GameService.DTOs;
using GameService.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace GameService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private readonly IGameRepository _gameRepository;

        public GameController(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateGame([FromForm]GameDTO gameDTO)
        {
            var response = await _gameRepository.CreateGame(gameDTO);
            if(response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpDelete("{gameId}")]
        public async Task<IActionResult> RemoveGame([FromRoute]Guid gameId)
        {
            var response = await _gameRepository.RemoveGame(gameId);
            if(response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllGames()
        {
            var response = await _gameRepository.GetAllGames();
            if(response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpGet("{categoryId}")]
        public async Task<IActionResult> GetGamesByCategoryId([FromRoute]Guid categoryId)
        {
            var response = await _gameRepository.GetGamesByCategory(categoryId);
            if(response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpPut("{gameId}")]
        public async Task<IActionResult> UpdateGame(UpdateGameDTO gameDTO, [FromRoute]Guid gameId)
        {
            var response = await _gameRepository.UpdateGame(gameDTO, gameId);
            if(response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

    }
}