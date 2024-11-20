using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Data;

public class DbInitializer
{
    public static async Task InitializeDb(WebApplication app)
    {
        // await DB.InitAsync("GameSearchDb", MongoClientSettings.FromConnectionString(app.Configuration.GetConnectionString("MongoDbConnection")));
        

        // await DB.Index<GameItem>()
        //     .Key(a => a.GameName, KeyType.Text)
        //     .Key(a => a.GameAuthor, KeyType.Text)
        //     .Key(a => a.GameDescription, KeyType.Text)
        //     .CreateAsync();
        try
    {
        // MongoDB ayarlarını bağlantı dizesinden al
        var settings = MongoClientSettings.FromConnectionString(app.Configuration.GetConnectionString("MongoDbConnection"));

        // GameSearchDb veritabanını başlat
        await DB.InitAsync("GameSearchDb", settings);

        // Test için koleksiyon ve veri ekle
        var testItem = new GameItem
        {
            GameName = "Sample Game",
            GameAuthor = "Sample Author",
            GameDescription = "Sample Description"
        };

        await DB.SaveAsync(testItem);
        Console.WriteLine("Veritabanı başarıyla başlatıldı ve test verisi eklendi.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Hata: {ex.Message}");
        throw;
    }
    }
    
}