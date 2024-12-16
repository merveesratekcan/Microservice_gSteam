using BasketService.Model;
using BasketService.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace BasketService.Controllers;
[ApiController]
[Route("[controller]")]

public class BasketController : ControllerBase
{
    private readonly IBasketRepository _basketRepository;
    public BasketController(IBasketRepository basketRepository)
    {
        _basketRepository = basketRepository;
    }
    [HttpPost]
    // [Authorize]
    public async Task<ActionResult> AddBasketItem(BasketModel model)
    {
        var response = await _basketRepository.AddBasket(model);
        return Ok(response);
    }
    [HttpGet("BasketItems")]
    
    public async Task<ActionResult> GetListItems()
    {
        var response = await _basketRepository.GetBasketItems();
        return Ok(response);
    }
    [HttpGet("BasketItem/{index}")]
    [Authorize]
   // Eğer index gönderilirse, o indexe sahip olan ürünü döndürüyor.routedan çekmek için [FromRoute] kullanılır.
    public async Task<ActionResult> GetBasketItem([FromRoute]long index)
    {
        var response = await _basketRepository.GetBasketItem(index);
        return Ok(response);
    }
    [HttpDelete("{index}")]
    [Authorize]
    public async Task<ActionResult> RemoveItem([FromRoute]long index)
    {
        var response = await _basketRepository.RemoveBasketItem(index);
        return Ok(response);
    }
    // [HttpPut("BasketItem/{index}")]
    // public async Task<ActionResult> UpdateItem(BasketModel model, long index)
    // {
    //     var response = await _basketRepository.UpdateBasketItem(model, index);
    //     return Ok(response);
    // }
    [HttpPost("Checkout")]
    [Authorize]
    public async Task<ActionResult> Checkout()
    {
        var response = await _basketRepository.Checkout();
        return Ok(response);
    }

   
}

    
   

