using MongoDB.Driver;
using MongoDB.Entities;

namespace SearchService.Data;

public class DbInitializer
{
    public static async Task InitializeDb(WebApplication app)
    {
        await DB.InitAsync("GameSearchDb", MongoClientSettings.FromConnectionString(app.Configuration.GetConnectionString("MongoDbConnection")));
    }
}