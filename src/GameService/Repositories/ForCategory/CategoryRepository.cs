using AutoMapper;
using GameService.Base;
using GameService.Data;
using GameService.DTOs;
using GameService.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameService.Repositories.ForCategory;

public class CategoryRepository : ICategoryRepository
{
    private readonly GameDbContext _context;
    private IMapper _mapper;
    private BaseResponseModel _baseResponseModel;
    public CategoryRepository(GameDbContext context,IMapper mapper,BaseResponseModel baseResponseModel)
    {       
        _context = context;
        _mapper = mapper;
        _baseResponseModel = baseResponseModel;
    }

    public async Task<BaseResponseModel> CreateCategories(CategoryDTO categoryDTO)
    {
        var objDTO = _mapper.Map<Category>(categoryDTO);
        await _context.Categories.AddAsync(objDTO);
        if (await _context.SaveChangesAsync() > 0)
        {
            _baseResponseModel.IsSuccess = true;
            _baseResponseModel.Message = "Category Created Successfully";
            _baseResponseModel.Data = objDTO;
            return _baseResponseModel;
        }
        _baseResponseModel.IsSuccess = false;
        return _baseResponseModel;
    }

    public async Task<BaseResponseModel> GetAllCategories()
    {
        List<Category> categories = await _context.Categories.ToListAsync();
        if(categories is not null)
        {
            _baseResponseModel.IsSuccess = true;
            _baseResponseModel.Message = "Categories Fetched Successfully";
            _baseResponseModel.Data = categories;
            return _baseResponseModel;
        }
        _baseResponseModel.IsSuccess = false;
        return _baseResponseModel;
    }

    public async Task<bool> RemoveCategory(Guid categoryId)
    {
        Category category = await _context.Categories.FindAsync(categoryId);
        if(category is not null)
        {
            _context.Categories.Remove(category);
            if (await _context.SaveChangesAsync() > 0)
            {
                return true;
            }
        }
        return false;
    }

    public async Task<BaseResponseModel> UpdateCategoryById(CategoryDTO categoryDTO, Guid categoryId)
    {
        Category category =await _context.Categories.FindAsync(categoryId);
        if(category is not null)
        {
            category.CategoryName = categoryDTO.CategoryName;
            category.CategoryDescription = categoryDTO.CategoryDescription;
            if (await _context.SaveChangesAsync() > 0)
            {
                _baseResponseModel.IsSuccess = true;
                _baseResponseModel.Message = "Category Updated Successfully";
                _baseResponseModel.Data = category;
                return _baseResponseModel;
            }

        }
        _baseResponseModel.IsSuccess = false;
        return _baseResponseModel;
    }
}

