using DiscountService.Data;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace DiscountService.Services;

public class GrpcDiscountService : GrpcDiscount.GrpcDiscountBase
{
   private readonly AppDbContext  _context;

    public GrpcDiscountService(AppDbContext context)
    {
        _context = context;
    }

    public override async Task<GrpcDiscountResponse> GetDiscount(GetDiscountRequest request, ServerCallContext context)
    {
        Console.WriteLine("Grpc Received call service started");
        var Discount = await _context.Discounts.FirstOrDefaultAsync(x => x.CouponCode == request.CouponCode);

        if (Discount == null)
        {
           
        }

        var response = new GrpcDiscountResponse
        {
            Discount= new GrpcDiscountModel
            {
               ExpireDate= Discount.ExpireDate.ToTimestamp(),
               CouponCode= Discount.CouponCode,
               DiscountAmount= Discount.DiscountAmount,
               GameId= Discount.GameId,
               UserId= Discount.UserId
                
            }
        };
        return response;
    }
}

