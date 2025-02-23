using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Iyzipay;
using Iyzipay.Model;
using Iyzipay.Request;
using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Entities;
using OrderService.Models;
using OrderService.Services.GrpcFolder;


namespace OrderService.Services;

    public class PaymentService : IPaymentService
    {
        private readonly ApplicationDbContext _context;
        private readonly GrpcMyGameClient _myGameClient;
        private string UserId;

    public PaymentService(IConfiguration _configuration, IHttpContextAccessor _contextAccessor, ApplicationDbContext context, GrpcMyGameClient myGameClient)
    {

        UserId = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        _context = context;
        _myGameClient = myGameClient;
    }

    public GrpcMyGameClient MyGameClient => _myGameClient;

    public async Task<bool> PayMyGames(PaymentForm model)
        {

            var result = await GetOrderByUserId(UserId);
            decimal price=0;
            foreach (var item in result)
            {
                price = price+item.Price;
            }
            Options options = new Options();
            options.ApiKey = Key.ApiKey;
            options.SecretKey = Key.SecretKey;
            options.BaseUrl = "https://sandbox-api.iyzipay.com";
                    
            CreatePaymentRequest request = new CreatePaymentRequest();
            request.Locale = Locale.TR.ToString();
            request.ConversationId = "123456789";
            request.Price = price.ToString();
            request.PaidPrice = price.ToString();
            request.Currency = Currency.TRY.ToString();
            request.Installment = 1;
            request.BasketId = "B67832";
            request.PaymentChannel = PaymentChannel.WEB.ToString();
            request.PaymentGroup = PaymentGroup.PRODUCT.ToString();

            PaymentCard paymentCard = new PaymentCard();
            paymentCard.CardHolderName = model.CardHolderName;
            paymentCard.CardNumber = model.CardNumber;
            paymentCard.ExpireMonth = model.ExpireMonth;
            paymentCard.ExpireYear =model.ExpireYear;
            paymentCard.Cvc = model.Cvc;
            paymentCard.RegisterCard = 0;
            request.PaymentCard = paymentCard;

            Buyer buyer = new Buyer();
            buyer.Id = "BY789";
            buyer.Name = "John";
            buyer.Surname = "Doe";
            buyer.GsmNumber = "+905350000000";
            buyer.Email = "email@email.com";
            buyer.IdentityNumber = "74300864791";
            buyer.LastLoginDate = "2015-10-05 12:43:35";
            buyer.RegistrationDate = "2013-04-21 15:12:09";
            buyer.RegistrationAddress = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
            buyer.Ip = "85.34.78.112";
            buyer.City = "Istanbul";
            buyer.Country = "Turkey";
            buyer.ZipCode = "34732";
            request.Buyer = buyer;

            Address shippingAddress = new Address();
            shippingAddress.ContactName = "Jane Doe";
            shippingAddress.City = "Istanbul";
            shippingAddress.Country = "Turkey";
            shippingAddress.Description = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
            shippingAddress.ZipCode = "34742";
            request.ShippingAddress = shippingAddress;

            Address billingAddress = new Address();
            billingAddress.ContactName = "Jane Doe";
            billingAddress.City = "Istanbul";
            billingAddress.Country = "Turkey";
            billingAddress.Description = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
            billingAddress.ZipCode = "34742";
            request.BillingAddress = billingAddress;

            List<BasketItem> basketItems = new List<BasketItem>();

            foreach (var item in result)
            {
                BasketItem firstBasketItem = new BasketItem();
                firstBasketItem.Id = item.GameId.ToString();
                firstBasketItem.Name = item.GameName;
                firstBasketItem.Category1 = "Games";
                firstBasketItem.Category2 = "Games";
                firstBasketItem.ItemType = BasketItemType.PHYSICAL.ToString();
                firstBasketItem.Price = item.Price.ToString();
                basketItems.Add(firstBasketItem);
            }
            

            
            request.BasketItems = basketItems;

            Payment payment = Payment.Create(request, options);
            if (payment.Status == "success")
            {
                foreach (var item in result)
                {
                    var isPaid= await PaidGameOrder(result);
                    var checkResult= _myGameClient.SaveMyGame(UserId, item.GameId.ToString());
                    if (!checkResult || !isPaid)
                    {
                        return false;
                    }
                   
                }
                return true;
                
            }
            return false;
       }
       private async Task<List<Order>> GetOrderByUserId(string userId)
       {
           
           //AsNoTracking:data sayısı az kullanımıcaz ama  ef de yaptığımız her adımı context tarafından belleğe alır.Birden fazla data olacağı için yönetilmesi için zaman kazandıracak.trak etmeyi durdurur.
           var result= await _context.Orders.Where(x => x.UserId == Guid.Parse(userId) && !x.IsPaid).ToListAsync();
           return result;
       }
       private async Task<bool> PaidGameOrder(List<Order> orders)
       {
           foreach (var item in orders)
           {
            var result = await _context.Orders.FirstOrDefaultAsync(x => x.OrderId == item.OrderId);
            result.IsPaid = true;
            await _context.SaveChangesAsync();
           }
           return true;
       }
  }
         
