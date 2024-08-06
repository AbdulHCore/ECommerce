using Catalog.Core.Entities;
using MongoDB.Driver;
using System.Text.Json;

namespace Catalog.Infrastructure.Data
{
    public static class ProductTypeContextSeed
    {
        public static void SeedData(IMongoCollection<ProductType> productTypes)
        {
            bool checkTypes = productTypes.Find(e => true).Any();
            string path = Path.Combine("Data", "SeedData", "types.json");
            //string path = "../Catalog.Infrastructure/Data/SeedData/types.json";
            if (!checkTypes)
            {
                var productTypesData = File.ReadAllText(path);
                var types = JsonSerializer.Deserialize<List<ProductType>>(productTypesData);
                if (types != null)
                {
                    foreach (var item in types)
                    {
                        productTypes.InsertOneAsync(item);
                    }
                }
            }
        }
    }
}
