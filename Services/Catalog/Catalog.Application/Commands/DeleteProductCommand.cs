using MediatR;

namespace Catalog.Application.Commands
{
    public class DeleteProductCommand : IRequest<bool>
    {
        public DeleteProductCommand(string id)
        {
            Id = id;
        }
        public string Id { get; set; }

    }
}
