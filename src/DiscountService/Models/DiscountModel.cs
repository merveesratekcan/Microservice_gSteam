namespace DiscountService.Models;

    public class DiscountModel
    {
        public int DiscountAmount { get; set; }
        public string CouponCode { get; set; }
        public string GameId { get; set; }
        public string? UserId { get; set; }
    }
 