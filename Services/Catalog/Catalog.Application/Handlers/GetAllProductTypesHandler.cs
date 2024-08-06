using Catalog.Application.Mapper;
using Catalog.Application.Queries;
using Catalog.Application.Responses;
using Catalog.Core.Repositories;
using MediatR;

namespace Catalog.Application.Handlers
{
    public class GetAllProductTypesHandler : IRequestHandler<GetAllProductTypesQuery, IList<ProductTypeResponse>>
    {
        private readonly IProductTypeRepository _productTypeRepository;
        public GetAllProductTypesHandler(IProductTypeRepository productTypeRepository) 
        { 
            _productTypeRepository = productTypeRepository;
        }
        public async Task<IList<ProductTypeResponse>> Handle(GetAllProductTypesQuery request, CancellationToken cancellationToken)
        {
            var types = await _productTypeRepository.GetAllTypes();
            var typesResponse = ProductMapper.Mapper.Map<IList<ProductTypeResponse>>(types);
            return typesResponse;
        }
    }
}
