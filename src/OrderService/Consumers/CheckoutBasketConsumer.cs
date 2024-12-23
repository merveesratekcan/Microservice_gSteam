using AutoMapper;
using Contracts;
using MassTransit;
using OrderService.Data;
using OrderService.Entities;

namespace OrderService.Consumers;

public class CheckoutBasketConsumer : IConsumer<CheckoutBasketModel>
{
    private readonly IMapper _mapper;
    private readonly ApplicationDbContext _context;

    public CheckoutBasketConsumer(IMapper mapper, ApplicationDbContext context)
    {
        _mapper = mapper;
        _context = context;
    }

    public async Task Consume(ConsumeContext<CheckoutBasketModel> context)
    {
        Console.WriteLine("Checkout basket consumer is called");
        var item=_mapper.Map<Order>(context.Message);
        await _context.Orders.AddAsync(item);
        await _context.SaveChangesAsync();

    }
}
