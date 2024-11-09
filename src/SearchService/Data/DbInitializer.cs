using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Data;

public class DbInitializer
{
    public static async Task InitializeDb(WebApplication app)
    {
        await DB.InitAsync("GameSearchDb", MongoClientSettings.FromConnectionString(app.Configuration.GetConnectionString("MongoDbConnection")));
        

        await DB.Index<GameItem>()
            .Key(a => a.GameName, KeyType.Text)
            .Key(a => a.GameAuthor, KeyType.Text)
            .Key(a => a.GameDescription, KeyType.Text)
            .CreateAsync();
    }
}