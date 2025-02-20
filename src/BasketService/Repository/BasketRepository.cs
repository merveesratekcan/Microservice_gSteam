using System.Security.Claims;
using AutoMapper;
using BasketService.Base;
using BasketService.Model;
using Contracts;
using DiscountService.Services;
using MassTransit;
using MassTransit.SagaStateMachine;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace BasketService.Repository
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDatabase _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public string connectionString;
        public string UserId;
        //constructor her teiklendiğinde redis bağlantısı yapılır,
        private readonly GrpcDiscountClient _grpcDiscountClient;
        //Masstrasit kullanarak event publish etmek için IPublishEndpoint kullanılır.Checkout işlemi gerçekleştiğinde bir event publish edilir.
        public IPublishEndpoint _publishEndpoint;

        private readonly IMapper _mapper;
        public BasketRepository(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IPublishEndpoint publishEndpoint, IMapper mapper, GrpcDiscountClient grpcDiscountClient)
        {
            connectionString = configuration.GetValue<string>("RedisDatabase");
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(connectionString);
            _db = redis.GetDatabase();
            _httpContextAccessor = httpContextAccessor;
            UserId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            _publishEndpoint = publishEndpoint;
            _mapper = mapper;
            _grpcDiscountClient = grpcDiscountClient;
        }


        public async Task<ResponseModel<bool>> AddBasket(BasketModel model)
        {
          
           ResponseModel<bool> responseModel=new ResponseModel<bool>();
           if(model is not null)
           {
              var convertType = JsonConvert.SerializeObject(model);
              await _db.ListRightPushAsync(UserId,convertType);
              responseModel.isSuccess=true;
              return responseModel;
           }
            return responseModel;
        }

        public async Task<ResponseModel<bool>> Checkout()
        {
            List<Checkout> checkouts=new List<Checkout>();
            ResponseModel<bool> responseModel=new ResponseModel<bool>();
             var response= await _db.ListRangeAsync(UserId);
            List<BasketModel> basketModel=new List<BasketModel>();
            foreach (var item in response)
            {
                Checkout _checkout=new Checkout();
                var objResult=JsonConvert.DeserializeObject<BasketModel>(item);
                _checkout.GameId=objResult.GameId;
                _checkout.GameName=objResult.GameName;
                _checkout.GameAuthor=objResult.GameAuthor;
                _checkout.Price=objResult.Price;
                _checkout.GameDescription=objResult.GameDescription;
                _checkout.UserId=Guid.Parse(UserId);
                checkouts.Add(_checkout);
            }
            if(checkouts.Count>0)
            {
                responseModel.isSuccess=true;
                foreach(var item in checkouts)
                {
                    try
                    {
                    await _publishEndpoint.Publish(_mapper.Map<CheckoutBasketModel>(item));
                    }
                    catch (System.Exception ex)
                    {
                        
                        throw ex;
                    }                   
                }
                await _db.KeyDeleteAsync(UserId);

               return responseModel;
            }
            responseModel.isSuccess=false;
            return responseModel;
        }

        public async Task<ResponseModel<BasketModel>> GetBasketItem(long index)
        {

            ResponseModel<BasketModel> responseModel=new ResponseModel<BasketModel>();
            var response = await _db.ListGetByIndexAsync(UserId, index);
            var objResult=JsonConvert.DeserializeObject<BasketModel>(response);
            responseModel.isSuccess=true;
            responseModel.Data=objResult;
            return responseModel;

        }

        public async Task<ResponseModel<List<BasketModel>>> GetBasketItems()
        {
            ResponseModel<List<BasketModel>> responseModel=new ResponseModel<List<BasketModel>>();
            if(!string.IsNullOrEmpty(UserId))
            {
               
            var response= await _db.ListRangeAsync(UserId);
            List<BasketModel> basketModel=new List<BasketModel>();
            foreach (var item in response)
            {
                var objResult=JsonConvert.DeserializeObject<BasketModel>(item);
                basketModel.Add(objResult);
            }
            if(basketModel.Count>0)
            {  
                    responseModel.isSuccess=true;
                    responseModel.Data=basketModel;
                    return responseModel;   
            }
           responseModel.isSuccess=false;
           return responseModel;
            }
            responseModel.isSuccess=false;
            responseModel.Message="Please before login your account";
            return responseModel;
        }

        public async Task<ResponseModel<bool>> RemoveBasketItem(long index)
        {
            ResponseModel<bool> responseModel=new ResponseModel<bool>();
           var willDeletedItem=await _db.ListGetByIndexAsync(UserId,index);
           await _db.ListRemoveAsync(UserId,willDeletedItem);
              responseModel.isSuccess=true;
                return responseModel;
            
        }

        public  async Task<ResponseModel<bool>> ImplementCoupon(long index,string couponCode)
        {
            ResponseModel<bool> responseModel=new ResponseModel<bool>();
            //grpc servisine bağlanarak coupon kodunu kontrol ederiz.Burada grpc devrede.
            var discount= _grpcDiscountClient.GetDiscount(couponCode);
            if(discount != null)
            {
                var response = await _db.ListGetByIndexAsync(UserId, index);
                var deserilizeObj=JsonConvert.DeserializeObject<BasketModel>(response);
                deserilizeObj.Price = deserilizeObj.Price-(deserilizeObj.Price * discount.DiscountAmount)/100;
                //redise set methodu ile güncellenen objeyi set ederiz.
                var serilizeObj=JsonConvert.SerializeObject(deserilizeObj);
                await _db.ListSetByIndexAsync(UserId,index,serilizeObj);
                responseModel.isSuccess=true;
                return responseModel;
            }
            responseModel.isSuccess=false;
            return responseModel;
        }

        //Bir kupon kudunun 1 kere kullanılmasını sağlamak için aşağıdaki kod bloğu yazılır.
        public async Task<ResponseModel<bool>> CheckCouponCode(string couponCode)
        {
            try
            {
                if(!string.IsNullOrEmpty(couponCode))
                {
                    var response = await _db.StringGetAsync(couponCode);
                    if(response.HasValue)
                    {
                        return new ResponseModel<bool>
                        {
                            isSuccess=false,
                            Message="This coupon code has already been used"
                        };
                    }
                   var key =  $"ImplementCoupon:{UserId}";
                   var isCouponCode = await _db.SetContainsAsync(key,couponCode);
                     if(isCouponCode)
                     {
                          return new ResponseModel<bool>
                          {
                            isSuccess=false,
                            Message="This coupon code has already been used"
                          };
                     }
                     var discount= _grpcDiscountClient.GetDiscount(couponCode);
                     if(discount == null)
                     {
                         return new ResponseModel<bool>
                         {
                             isSuccess=false,
                             Message="This coupon code is not valid"
                         };
                     }
                     var values=await _db.ListLengthAsync(UserId);
                     if(values == 0)
                     {
                         return new ResponseModel<bool>
                         {
                             isSuccess=false,
                             Message="Please add a product to your basket"
                         };
                     }
                     for(var index=0;index<values;index++)
                     {
                         var response2 = await _db.ListGetByIndexAsync(UserId, index);
                         var ticket = JsonConvert.DeserializeObject<BasketModel>(response2);
                         ticket!.Price = ticket.Price - (ticket.Price * discount.DiscountAmount) / 100;
                         await _db.ListSetByIndexAsync(UserId, index, JsonConvert.SerializeObject(ticket));
                         
                     }
                     await _db.SetAddAsync(key,couponCode);
                     return new ResponseModel<bool>
                     {
                         isSuccess=true,
                         Message="Coupon code has been successfully implemented"
                     };
                     
                }
                return new ResponseModel<bool>
                {
                    isSuccess=false,
                    Message="Please enter a coupon code"
                };
            }
                catch(Exception ex)
                {
                    return new ResponseModel<bool>
                    {
                        isSuccess=false,
                        Message=ex.Message
                    };
                }
            }
        }
}

            
