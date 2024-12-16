using BasketService.Base;
using BasketService.Model;

namespace BasketService.Repository;

    public interface IBasketRepository
    {
        Task<ResponseModel<bool>> AddBasket(BasketModel model);
        //long index göndermek şu işe yarıyor. Eğer index gönderilirse, o indexe sahip olan ürünü döndürüyor.
        //Long-Yani 1,2,3,4,5 gibi sayılar.
        Task<ResponseModel<BasketModel>> GetBasketItem(long index);
        Task<ResponseModel<List<BasketModel>>> GetBasketItems();
        Task<ResponseModel<bool>> RemoveBasketItem(long index);
        // Task<ResponseModel<BasketModel>> UpdateBasketItem(BasketModel model,long index);
        Task<ResponseModel<bool>> Checkout();

    }
