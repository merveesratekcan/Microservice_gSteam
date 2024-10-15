using GameService.Base;
using GameService.Entities;

namespace GameService.Entities
{
    public class Category : BaseModel
    {
         public string CategoryName { get; set; }
         public string CategoryDescription { get; set; }
         public ICollection<Game> Games { get; set; }

    }
}