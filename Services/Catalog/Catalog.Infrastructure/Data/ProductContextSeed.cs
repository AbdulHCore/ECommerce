
using Catalog.Core.Entities;
using MongoDB.Driver;
using System.Text.Json;

namespace Catalog.Infrastructure.Data
{
    public static class ProductContextSeed
    {
        public static void SeedData(IMongoCollection<Product> products)
        {
            bool checkProducts = products.Find(e => true).Any();
            string path = Path.Combine("Data", "SeedData", "products.json");
            //string path = "../Catalog.Infrastructure/Data/SeedData/products.json";
            if (!checkProducts)
            {
                var productsData = File.ReadAllText(path);
                var productsSeed = JsonSerializer.Deserialize<List<Product>>(productsData);
                if (productsSeed != null)
                {
                    foreach (var item in productsSeed)
                    {
                        products.InsertOneAsync(item);
                    }
                }
            }
        }
    }
}
