using Basket.Application.Commands;
using Basket.Application.GrpcService;
using Basket.Application.Mappers;
using Basket.Application.Queries;
using Basket.Application.Responses;
using Basket.Core.Entities;
using EventBus.Messages.Events;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Basket.API.Controllers
{
    public class BasketController : ApiController
    {
        private readonly IMediator _mediator;
        private readonly IPublishEndpoint _publishEndpoint;
        
        public BasketController(IMediator mediator, IPublishEndpoint publishEndPoint) 
        { 
            _mediator = mediator;            
            _publishEndpoint = publishEndPoint;
        }

        [HttpGet]
        [Route("[action]/{userName}", Name ="GetBasketByUserName")]
        [ProducesResponseType (typeof(ShoppingCartResponse), (int)HttpStatusCode.OK)]            
        public async Task<ActionResult<ShoppingCartResponse>> GetBasketByUserName(string userName)
        {
            var query = new GetBasketByUserNameQuery(userName);
            var basket = await _mediator.Send(query);
            return Ok(basket);
        }

        [HttpPost]
        [ProducesResponseType (typeof(ShoppingCartResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCartResponse>> CreateBasket([FromBody] CreateShoppingCartCommand shoppingCart)
        {            
            var basket = await _mediator.Send(shoppingCart);
            return Ok(basket);
        }

        [HttpDelete]
        [ProducesResponseType(typeof(Unit), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Unit>> DeleteBasket(string userName)
        {
            var command = new DeleteBasketByUserNameCommand(userName);
            var unit = await _mediator.Send(command);
            return Ok(unit);
        }

        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> Checkout([FromBody] BasketCheckout basketCheckout)
        {
            var query = new GetBasketByUserNameQuery(basketCheckout.UserName);
            var basket = await _mediator.Send(query);
            if (basket == null)
            {
                return BadRequest();
            }

            //publish the message to RabbitMQ
            var eventMsg = BasketMapper.Mapper.Map<BasketCheckoutEvent>(basketCheckout);
            eventMsg.TotalPrice = basket.TotalPrice;
            await _publishEndpoint.Publish(eventMsg);

            //remove the basket once the message published successfully.
            var deleteCommand = new DeleteBasketByUserNameCommand(basketCheckout.UserName);
            await _mediator.Send(deleteCommand);

            return Accepted();
        }
    }
}
