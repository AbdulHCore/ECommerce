﻿using Discount.Application.Queries;
using Discount.Core.Repositories;
using Discount.Grpc.Protos;
using MediatR;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Discount.Application.Handlers
{
    public class GetDiscountQueryHandler : IRequestHandler<GetDiscountQuery, CouponModel>
    {
        private readonly IDiscountRepository _discountRepository;
        private readonly ILogger _logger;
        public GetDiscountQueryHandler(IDiscountRepository discountRepository, ILogger<GetDiscountQueryHandler> logger)
        {
            _discountRepository = discountRepository;
            _logger = logger;
        }
        public async Task<CouponModel> Handle(GetDiscountQuery request, CancellationToken cancellationToken)
        {
            var coupon = await _discountRepository.GetDiscount(request.ProductName);
            if (coupon == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Discount for the ProductName= {request.ProductName} not found."));
            }

            CouponModel couponModel = new CouponModel 
            { 
                Amount = coupon.Amount, 
                ProductName = coupon.ProductName, 
                Description = coupon.Description, 
                Id = coupon.Id 
            };
            _logger.LogInformation($"Coupon for the {request.ProductName} is fetched");
            return couponModel;
        }
    }
}
