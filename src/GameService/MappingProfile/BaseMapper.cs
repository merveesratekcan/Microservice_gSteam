using AutoMapper;
using GameService.DTOs;
using GameService.Entities;

namespace GameService.MappingProfile;

    public class BaseMapper : Profile
    {
        public BaseMapper()
        {
            CreateMap<Category, CategoryDTO>().ReverseMap();
            CreateMap<Game, GameDTO>().ReverseMap();
        }
        
    }
