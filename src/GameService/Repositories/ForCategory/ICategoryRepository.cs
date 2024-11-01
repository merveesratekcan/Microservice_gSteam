using GameService.Base;
using GameService.DTOs;
using GameService.Entities;

namespace GameService.Repositories.ForCategory;

public interface ICategoryRepository
{
   Task<BaseResponseModel> CreateCategories(CategoryDTO categoryDTO);
   Task<bool> RemoveCategory(Guid categoryId);
   Task<BaseResponseModel> UpdateCategoryById(CategoryDTO categoryDTO, Guid categoryId);
   Task<BaseResponseModel> GetAllCategories();
}