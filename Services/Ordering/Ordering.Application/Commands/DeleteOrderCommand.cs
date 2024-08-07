using MediatR;

namespace Ordering.Application.Commands
{
    public class DeleteOrderCommand : IRequest<Unit>
    {
        public DeleteOrderCommand(int orderId)
        {
            Id = orderId;
        }
        public int Id { get; set; }

    }
}
