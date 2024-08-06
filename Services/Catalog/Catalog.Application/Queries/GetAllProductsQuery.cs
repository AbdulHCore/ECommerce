using Catalog.Application.Responses;
using Catalog.Core.Specs;
using MediatR;

namespace Catalog.Application.Queries
{
    public class GetAllProductsQuery : IRequest<Pagination<ProductResponse>>
    {

        public GetAllProductsQuery(CatalogSpecParams catalogSpecParams)
        { 
            CatalogSpec = catalogSpecParams;
        }

        public CatalogSpecParams CatalogSpec { get; set; }
    }
}
