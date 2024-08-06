using Basket.Core.Entities;

namespace Basket.Core.Repositories
{
    public interface IBasektRepository
    {
        Task<ShoppingCart> GetBasket(string userName);
        
        /// <summary>
        /// CRUD Methods
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        Task<ShoppingCart> UpdateBasket(ShoppingCart shoppingCart);
        Task DeleteBasket(string userName);

    }
}
