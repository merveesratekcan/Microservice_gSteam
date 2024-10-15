using GameService.Base;
using GameService.Entities;

namespace GameService.Repositories.ForCategory;

public interface ICategoryRepository
{
   Task<BaseResponseModel> CreateCategories();
}