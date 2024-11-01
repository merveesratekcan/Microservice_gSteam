using AutoMapper;
using GameService.Base;
using GameService.Data;
using GameService.DTOs;
using GameService.Entities;
using GameService.Services;

namespace GameService.Repositories;

public class GameRepository : IGameRepository
{
    private readonly GameDbContext _context;
    private readonly IMapper _mapper;
    private readonly IFileService _fileService;
    private readonly BaseResponseModel _baseResponseModel;

    public GameRepository(GameDbContext context, IMapper mapper, IFileService fileService, BaseResponseModel baseResponseModel)
    {
        _context = context;
        _mapper = mapper;
        _fileService = fileService;
        _baseResponseModel = baseResponseModel;
    }

    public async Task<BaseResponseModel> CreateGame(GameDTO gameDTO)
    {
        if(gameDTO.File.Length > 0)
        {
           string videoUrl = await _fileService.UploadVideo(gameDTO.File);
           var obj = _mapper.Map<Game>(gameDTO);
           obj.VideoUrl = videoUrl;
           await _context.Games.AddAsync(obj);
           if(await _context.SaveChangesAsync() > 0)
           {
               _baseResponseModel.IsSuccess=true;
                _baseResponseModel.Message = "Game created successfully";
                _baseResponseModel.Data = obj;
                return _baseResponseModel;
           }
        }
        _baseResponseModel.IsSuccess=false;
        return _baseResponseModel;

    }
}