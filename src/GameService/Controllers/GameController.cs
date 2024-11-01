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
    }
}