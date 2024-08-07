using Microsoft.Extensions.Logging;
using Ordering.Core.Entities;

namespace Ordering.Infrastructure.Data
{
    public class OrderContextSeed
    {
        public static async Task SeedAsync(OrderContext context, ILogger<OrderContextSeed> logger)
        {
            if (!context.Orders.Any())
            {
                context.Orders.AddRange(GetOrders());
                await context.SaveChangesAsync();
                logger.LogInformation($"Ordering Database: {typeof(OrderContextSeed).Name} Seeded!"); 
            }
        }

        private static IEnumerable<Order> GetOrders()
        {
            return new List<Order>
            {
                new Order {
                    UserName = "abdul",
                    FirstName = "Abdul",
                    LastName = "Hakeem",
                    EmailAddress = "abdul.hakeem@sample.com",
                    AddressLine = "London",
                    Country = "Uk",
                    TotalPrice = 750,
                    State = "Wycombe",
                    ZipCode = "HP13 5DB",

                    CardName = "Visa",
                    CardNumber = "1234567890123456",
                    CreatedBy = "AbdulH",
                    Expiration = "12/25",
                    Cvv = "123",
                    PaymentMethod = 1,
                    LastModifiedBy = "Abdul",
                    LastModifiedDate = new DateTime()
                }
            };
        }
    }
}
