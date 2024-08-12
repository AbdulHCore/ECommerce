using AutoMapper;
using EventBus.Messages.Events;
using Ordering.Application.Commands;
using Ordering.Application.Responses;
using Ordering.Core.Entities;

namespace Ordering.Application.Mappers
{
    public class OrderMappingProfile : Profile
    {
        public OrderMappingProfile()
        {
            CreateMap<Order, OrderResponse>().ReverseMap();
            CreateMap<Order, CheckoutOrderCommand>().ReverseMap();            
            CreateMap<Order, UpdateOrderCommand>().ReverseMap();   
            CreateMap<CheckoutOrderCommand, BasketCheckoutEvent>().ReverseMap();
            
            //V2 Mappings:
            CreateMap<Order, CheckoutOrderCommandV2>().ReverseMap();
            CreateMap<CheckoutOrderCommandV2, BasketCheckoutEventV2>().ReverseMap();
        }
    }
}
