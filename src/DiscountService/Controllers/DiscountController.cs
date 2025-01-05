using DiscountService.Models;
using DiscountService.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiscountService.Controllers;

[ApiController]
[Route("[controller]")]
public class DiscountController : ControllerBase
{
    private readonly IDiscountRepository _discountRepository;
    

    public DiscountController(IDiscountRepository discountRepository)
    {
        _discountRepository = discountRepository;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateDiscount([FromBody] DiscountModel model)
    {
        var response=await _discountRepository.CreateDiscount(model);
        return Ok(response);
    }
}