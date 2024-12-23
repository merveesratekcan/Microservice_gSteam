using System.Security.Claims;
using AutoMapper;
using BasketService.Base;
using BasketService.Model;
using Contracts;
using MassTransit;
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

        //Masstrasit kullanarak event publish etmek için IPublishEndpoint kullanılır.Checkout işlemi gerçekleştiğinde bir event publish edilir.
        public IPublishEndpoint _publishEndpoint;

        private readonly IMapper _mapper;
        public BasketRepository(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IPublishEndpoint publishEndpoint, IMapper mapper)
        {
            connectionString = configuration.GetValue<string>("RedisDatabase");
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(connectionString);
            _db = redis.GetDatabase();
            _httpContextAccessor = httpContextAccessor;
            UserId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            _publishEndpoint = publishEndpoint;
            _mapper = mapper;
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
                    await _publishEndpoint.Publish(_mapper.Map<CheckoutBasketModel>(item));
                }
               
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

        // public async Task<ResponseModel<bool>> UpdateBasketItem(long index, BasketModel model)
        // {
        //     ResponseModel<bool> responseModel=new ResponseModel<bool>();
        //     var convertType = JsonConvert.SerializeObject(model);
        //     await _db.ListSetByIndexAsync(UserId,index,convertType);
        //     responseModel.isSuccess=true;
        //     return responseModel;
        // }
    }
}