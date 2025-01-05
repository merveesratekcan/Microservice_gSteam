using System.Security.Claims;
using DiscountService.Data;
using DiscountService.Entities;
using DiscountService.Models;
using DiscountService.Services;

namespace DiscountService.Repository;

public class DiscountRepository : IDiscountRepository
{
    private readonly AppDbContext _context;
    private readonly GrpcGameClient _grpcGameClient;
    private string UserId;

    public DiscountRepository(AppDbContext context, GrpcGameClient grpcGameClient, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _grpcGameClient = grpcGameClient;
        UserId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    public async Task<bool> CreateDiscount(DiscountModel model)
    {
        if(model != null)
        {
            var game=_grpcGameClient.GetGame(model.GameId,UserId);
            if(string.IsNullOrEmpty(game.GameName))
            {
                Discount discount = new Discount
                {
                    CouponCode = model.CouponCode,
                    DiscountAmount = model.DiscountAmount,
                    GameId = model.GameId,
                    UserId = model.UserId
                };
                await _context.Discounts.AddAsync(discount);
                if (await _context.SaveChangesAsync() > 0)
                {
                    return true;
                }
            }
          
        }
        return false;
    }
}
