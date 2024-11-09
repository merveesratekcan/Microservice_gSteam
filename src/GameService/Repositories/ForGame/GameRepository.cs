using AutoMapper;
using Contracts;
using GameService.Base;
using GameService.Data;
using GameService.DTOs;
using GameService.Entities;
using GameService.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace GameService.Repositories;

public class GameRepository : IGameRepository
{
    private readonly GameDbContext _context;
    private readonly IMapper _mapper;
    private readonly IFileService _fileService;
    private readonly BaseResponseModel _baseResponseModel;
    private readonly IPublishEndpoint _publishEndpoint;

    public GameRepository(GameDbContext context, IMapper mapper, IFileService fileService, BaseResponseModel baseResponseModel, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _mapper = mapper;
        _fileService = fileService;
        _baseResponseModel = baseResponseModel;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<BaseResponseModel> CreateGame(GameDTO gameDTO)
    {
        if(gameDTO.File.Length > 0)
        {
           string videoUrl = await _fileService.UploadVideo(gameDTO.File);
           var objDTO = _mapper.Map<Game>(gameDTO);
           objDTO.VideoUrl = videoUrl;
           await _context.Games.AddAsync(objDTO);
           await _publishEndpoint.Publish(_mapper.Map<GameCreated>(objDTO));
           if(await _context.SaveChangesAsync() > 0)
           {
            _baseResponseModel.IsSuccess=true;
            _baseResponseModel.Message = "Game created successfully";
            _baseResponseModel.Data = objDTO;
             return _baseResponseModel;
           }
        }
        _baseResponseModel.IsSuccess=false;
        return _baseResponseModel;

    }

    public async Task<BaseResponseModel> GetAllGames()
    {
       List<Game> games = await _context.Games.Include(x=>x.Category).Include(x=>x.GameImages).ToListAsync();
       if(games is not null)
       {
              _baseResponseModel.IsSuccess=true;
              _baseResponseModel.Data = games;
              return _baseResponseModel;
       }
         _baseResponseModel.IsSuccess=false;
        return _baseResponseModel;
    }

    public async Task<BaseResponseModel> GetGamesByCategory(Guid categoryId)
    {
        List<Game> games = await _context.Games.Where(x=>x.CategoryId == categoryId).ToListAsync();
        if(games is not null)
        {
              _baseResponseModel.IsSuccess=true;
              _baseResponseModel.Data = games;
              return _baseResponseModel;
        }
         _baseResponseModel.IsSuccess=false;
        return _baseResponseModel;
    }

    public async Task<BaseResponseModel> RemoveGame(Guid gameId)
    {
        Game game = await _context.Games.FindAsync(gameId);
        if(game is not null)
        {
            _context.Games.Remove(game);
            await _publishEndpoint.Publish<GameDeleted>(new {Id=gameId.ToString()});
            if(await _context.SaveChangesAsync() > 0){
                _baseResponseModel.Data = game;
                _baseResponseModel.IsSuccess=true;
                _baseResponseModel.Message = "Game removed successfully";
                return _baseResponseModel;
            }            
        }
        _baseResponseModel.IsSuccess=false;
        return _baseResponseModel;
    }

    public async Task<BaseResponseModel> UpdateGame(UpdateGameDTO gameDTO, Guid gameId)
    {
        Game game = await _context.Games.FindAsync(gameId);
        if(game is not null)
        {
            game.GameName = gameDTO.GameName;
            game.GameAuthor = gameDTO.GameAuthor;
            game.Price = gameDTO.Price;
            game.GameDescription = gameDTO.GameDescription;
            game.MinimumSystemRequirement = gameDTO.MinimumSystemRequirement;
            game.RecommendedSystemRequirement = gameDTO.RecommendedSystemRequirement;
            //game.CategoryId = gameDTO.CategoryId;
            await _publishEndpoint.Publish(_mapper.Map<GameUpdated>(game));
            if(await _context.SaveChangesAsync() > 0)
            {
                _baseResponseModel.Data = game;
                _baseResponseModel.IsSuccess=true;
                return _baseResponseModel;
            }
        }
        _baseResponseModel.IsSuccess=false;
        return _baseResponseModel;
    }
}