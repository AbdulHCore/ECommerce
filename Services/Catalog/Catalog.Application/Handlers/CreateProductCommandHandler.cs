
using Catalog.Application.Commands;
using Catalog.Application.Mapper;
using Catalog.Application.Responses;
using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using MediatR;

namespace Catalog.Application.Handlers
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductResponse>
    {
        private readonly IProductRepository _productRepository;
        public CreateProductCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        public async Task<ProductResponse> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var ProductEntity = ProductMapper.Mapper.Map<Product>(request);
            if (ProductEntity is null)
            {
                throw new ApplicationException("There is Issue with Command to Response mapping for New Product.");
            }
            var newProduct = await _productRepository.CreateProduct(ProductEntity);
            var newProductResponse = ProductMapper.Mapper.Map<ProductResponse>(newProduct);
            return newProductResponse;
        }
    }
}
