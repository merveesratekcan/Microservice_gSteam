using BasketService.Base;
using BasketService.Model;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace BasketService.Repository
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDatabase _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public string connectionString;
        //constructor her teiklendiğinde redis bağlantısı yapılır,
        public BasketRepository(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            connectionString = configuration.GetValue<string>("RedisDatabase");
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(connectionString);
            _db = redis.GetDatabase();
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task<ResponseModel<bool>> AddBasket(BasketModel model)
        {
           var user=_httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x=>x.Type=="username");
           ResponseModel<bool> responseModel=new ResponseModel<bool>();
           if(model is not null)
           {
              var convertType = JsonConvert.SerializeObject(model);
              await _db.ListRightPushAsync("user",convertType);
              responseModel.isSuccess=true;
              return responseModel;
           }
            return responseModel;
        }

        public async Task<ResponseModel<BasketModel>> GetBasketItem(long index)
        {

            ResponseModel<BasketModel> responseModel=new ResponseModel<BasketModel>();
            var response = await _db.ListGetByIndexAsync("Basket", index);
            var objResult=JsonConvert.DeserializeObject<BasketModel>(response);
            responseModel.isSuccess=true;
            responseModel.Data=objResult;
            return responseModel;

        }

        public async Task<ResponseModel<List<BasketModel>>> GetBasketItems()
        {
            ResponseModel<List<BasketModel>> responseModel=new ResponseModel<List<BasketModel>>();
            var response= await _db.ListRangeAsync("Basket");
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

        public async Task<ResponseModel<bool>> RemoveBasketItem(long index)
        {
            ResponseModel<bool> responseModel=new ResponseModel<bool>();
           var willDeletedItem=await _db.ListGetByIndexAsync("Basket",index);
           await _db.ListRemoveAsync("Basket",willDeletedItem);
              responseModel.isSuccess=true;
                return responseModel;
            
        }

        // public async Task<ResponseModel<bool>> UpdateBasketItem(long index, BasketModel model)
        // {
        //     ResponseModel<bool> responseModel=new ResponseModel<bool>();
        //     var convertType = JsonConvert.SerializeObject(model);
        //     await _db.ListSetByIndexAsync("Basket",index,convertType);
        //     responseModel.isSuccess=true;
        //     return responseModel;
        // }
    }
}