using MediatR;

namespace Basket.Application.Commands
{
    public class DeleteBasketByUserNameCommand : IRequest<Unit>
    {
        public DeleteBasketByUserNameCommand(string userName)
        {
            UserName = userName;
        }
        public string UserName { get; set; }

    }
}
