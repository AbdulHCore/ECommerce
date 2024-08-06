using Basket.Application.Commands;
using Basket.Application.Queries;
using Basket.Application.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Basket.API.Controllers
{
    public class BasketController : ApiController
    {
        private readonly IMediator _mediator;
        public BasketController(IMediator mediator) 
        { 
            _mediator = mediator;  
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
    }
}
