
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Commands;
using Ordering.Core.Entities;
using Ordering.Core.Repositories;

namespace Ordering.Application.Handlers
{
    public class CheckoutOrderCommandHandlerV2 : IRequestHandler<CheckoutOrderCommandV2, int>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public CheckoutOrderCommandHandlerV2(IOrderRepository orderRepository, IMapper mapper, ILogger<CheckoutOrderCommandHandlerV2> logger)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<int> Handle(CheckoutOrderCommandV2 request, CancellationToken cancellationToken)
        {
            var order = _mapper.Map<Order>(request);
            var result = await _orderRepository.AddAsync(order);
            _logger.LogInformation($"Order {result.Id} Successfully generatedon V2.");
            return result.Id;
        }
    }
}
