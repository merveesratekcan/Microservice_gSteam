using GameService.DTOs;
using GameService.Repositories.ForCategory;
using Microsoft.AspNetCore.Mvc;

namespace GameService.Controllers;

[ApiController]
[Route("[controller]")]

public class CategoryController:ControllerBase
{
    public ICategoryRepository _categoryRepository;
    public CategoryController(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }
    [HttpPost]
    public async Task<ActionResult> CreateCategories(CategoryDTO categoryDTO)
    {
        var response = await _categoryRepository.CreateCategories(categoryDTO);
        return Ok(response);
    }
   [HttpDelete("{categoryId}")]
    public async Task<ActionResult> RemoveCategory([FromRoute] Guid categoryId)
    {
        var response = await _categoryRepository.RemoveCategory(categoryId);
        if (response)
        {
            return Ok("Category Removed Successfully");
        }
        return NotFound("Category Not Found");
    }
    [HttpPut("{categoryId}")]
    public async Task<ActionResult> UpdateCategoryById(CategoryDTO categoryDTO,Guid categoryId)
    {
        var response = await _categoryRepository.UpdateCategoryById(categoryDTO, categoryId);
        return Ok(response);
    }
    [HttpGet]
    public async Task<ActionResult> GetAllCategories()
    {
        var response = await _categoryRepository.GetAllCategories();
        return Ok(response);
    }

    
}

