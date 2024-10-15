using GameService.Base;
using GameService.Entities;

namespace GameService.Entities
{
    public class Game : BaseModel
    {
        public string GameName { get; set; }
        public string GameAuthor { get; set; }
        public decimal Price { get; set; }
        public string VideoUrl { get; set; }
        public List<string> ImageUrl { get; set; } = new List<string>();
        public string GameDescription { get; set; }
        public string MinimumSystemRequirement { get; set; }
        public string RecommendedSystemRequirement { get; set; }
        public Guid CategoryId { get; set; }
        public Category Category { get; set; }
    }
}