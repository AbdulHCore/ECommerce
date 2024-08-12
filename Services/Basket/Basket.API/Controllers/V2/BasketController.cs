using Asp.Versioning;
using Basket.Application.Commands;
using Basket.Application.Mappers;
using Basket.Application.Queries;
using Basket.Core.Entities;
using EventBus.Messages.Events;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Basket.API.Controllers.V2
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("2")]
    public class BasketController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<BasketController> _logger;

        public BasketController(IMediator mediator, IPublishEndpoint publishEndPoint, ILogger<BasketController> logger)
        {
            _mediator = mediator;
            _publishEndpoint = publishEndPoint;
            _logger = logger;
        }

        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> Checkout([FromBody] BasketCheckoutV2 basketCheckout)
        {
            var query = new GetBasketByUserNameQuery(basketCheckout.UserName);
            var basket = await _mediator.Send(query);
            if (basket == null)
            {
                return BadRequest();
            }

            //publish the message to RabbitMQ
            var eventMsg = BasketMapper.Mapper.Map<BasketCheckoutEventV2>(basketCheckout);
            eventMsg.TotalPrice = basket.TotalPrice;
            await _publishEndpoint.Publish(eventMsg);
            _logger.LogInformation($"Checkout Message publisehd for {basketCheckout.UserName} with V2 endpoint");


            //remove the basket once the message published successfully.
            var deleteCommand = new DeleteBasketByUserNameCommand(basketCheckout.UserName);
            await _mediator.Send(deleteCommand);

            return Accepted();
        }
    }
}
