
using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using Catalog.Core.Specs;
using Catalog.Infrastructure.Data;
using MongoDB.Driver;

namespace Catalog.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository, IBrandRepository, IProductTypeRepository
    {
        public readonly ICatalogContext _catalogContext;
        public ProductRepository(ICatalogContext catalogContext)
        {
            _catalogContext = catalogContext;
        }

        async Task<Product> IProductRepository.GetProduct(string id)
        {
            return await _catalogContext
                .Products
                .Find(e => e.Id == id)
                .FirstOrDefaultAsync();
        }

        async Task<IEnumerable<Product>> IProductRepository.GetProductByBrand(string name)
        {
            return await _catalogContext
               .Products
               .Find(e => e.Brands.Name.ToLower() == name.ToLower())
               .ToListAsync();
        }

        async Task<IEnumerable<Product>> IProductRepository.GetProductByName(string name)
        {
            return await _catalogContext
               .Products
               .Find(e => e.Name.ToLower() == name.ToLower())
               .ToListAsync();
        }

        async Task<Pagination<Product>> IProductRepository.GetProducts(CatalogSpecParams catalogSpecParams)
        {
            var builder = Builders<Product>.Filter;
            var filter = builder.Empty;
            if (!string.IsNullOrEmpty(catalogSpecParams.Search))
                filter &= builder.Where(p => p.Name.ToLower().Contains(catalogSpecParams.Search.ToLower()));
            if(!string.IsNullOrEmpty(catalogSpecParams.BrandId))
                filter &= builder.Eq(p=> p.Brands.Id, catalogSpecParams.BrandId);
            if (!string.IsNullOrEmpty(catalogSpecParams.TypeId))
                filter &= builder.Eq(p => p.Types.Id, catalogSpecParams.TypeId);

            var totalItems = await _catalogContext.Products.CountDocumentsAsync(filter);
            var data = await DataFilterAndSort(catalogSpecParams, filter);            

            return new Pagination<Product>(catalogSpecParams.PageIndex, catalogSpecParams.PageSize, totalItems, data);
        }

        async Task<bool> IProductRepository.UpdateProduct(Product product)
        {
            var updateProduct = await _catalogContext.Products.ReplaceOneAsync(e => e.Id == product.Id, product);
            return updateProduct.IsAcknowledged && updateProduct.ModifiedCount > 0;
        }
        async Task<Product> IProductRepository.CreateProduct(Product product)
        {
            await _catalogContext.Products.InsertOneAsync(product);
            return product;
        }

        async Task<bool> IProductRepository.DeleteProduct(string id)
        {
            var deleteProduct = await _catalogContext.Products.DeleteOneAsync(e=> e.Id == id);
            return deleteProduct.IsAcknowledged && deleteProduct.DeletedCount > 0;
        }

        async Task<IEnumerable<ProductBrand>> IBrandRepository.GetAllBrands()
        {
            return await _catalogContext.Brands.Find(e => true).ToListAsync();
        }

        async Task<IEnumerable<ProductType>> IProductTypeRepository.GetAllTypes()
        {
            return await _catalogContext.Types.Find(e => true).ToListAsync();
        }

        private async Task<IReadOnlyList<Product>> DataFilterAndSort(CatalogSpecParams catalogSpecParams, FilterDefinition<Product> filter)
        {
            var sortDefn = Builders<Product>.Sort.Ascending("Name");
            if (!string.IsNullOrEmpty(catalogSpecParams.Sort))
            {
                switch (catalogSpecParams.Sort)
                {
                    case "priceDesc":
                        sortDefn = Builders<Product>.Sort.Descending(e => e.Price); break;
                    case "priceAsc":
                        sortDefn = Builders<Product>.Sort.Ascending(e => e.Price); break;
                    default:
                        sortDefn = Builders<Product>.Sort.Ascending(e => e.Name); break;
                }
            }
            return await _catalogContext
                .Products
                .Find(filter)
                .Sort(sortDefn)
                .Skip((catalogSpecParams.PageIndex - 1) * catalogSpecParams.PageSize)
                .Limit(catalogSpecParams.PageSize)
                .ToListAsync();

        }
    }
}
