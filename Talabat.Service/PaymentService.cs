using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Entities.OrderAggregate;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Services.Contract;
using Talabat.Core.Specifications.OrderSpecs;
using Product = Talabat.Core.Entities.Product;

namespace Talabat.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IBasketRepository _basketRepo;

        public PaymentService(
            IUnitOfWork unitOfWork,
            IConfiguration configuration,
            IBasketRepository basketRepo
            )
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _basketRepo = basketRepo;
        }

        public async Task<CustomerBasket?> CreateOrUpdatePaymentIntent(string basketId)
        {
            StripeConfiguration.ApiKey = _configuration["StripeSettings:Secretkey"];

            var basket = await _basketRepo.GetBasketAsync( basketId);

            if (basket is null) return null;

            var shippingPrice = 0m;

            if(basket.DeliveryMethodId.HasValue)
            {
                var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(basket.DeliveryMethodId.Value);
                shippingPrice = deliveryMethod.Cost;
                basket.ShippingPrice = shippingPrice;
            }

            if (basket?.Items.Count > 0)
            {
                foreach (var item in basket.Items)
                {
                    var product = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);
                    if (item.Price != product?.Price)
                        item.Price = product.Price;

                }
            }

            PaymentIntentService paymentIntentService = new PaymentIntentService();

            PaymentIntent paymentIntent;

            if(string.IsNullOrEmpty( basket.PaymentIntentId))
            {
                var CreateOptions = new PaymentIntentCreateOptions()
                {
                    Amount = (long) basket.Items.Sum(item => item.Price * item.Quantity * 100) + (long) shippingPrice * 100,
                    Currency = "usd",
                    PaymentMethodTypes = new List<string>() { "card"} 
                };

                paymentIntent = await paymentIntentService.CreateAsync(CreateOptions);

                basket.PaymentIntentId = paymentIntent.Id;
                basket.ClientSecret = paymentIntent.ClientSecret;
            }
            else
            {
                var updateOptions = new PaymentIntentUpdateOptions() 
                {
                    Amount = (long)basket.Items.Sum(item => item.Price * item.Quantity * 100) + (long)shippingPrice * 100
                };

                await paymentIntentService.UpdateAsync(basket.PaymentIntentId, updateOptions);
            }

            var updatedBasket = await _basketRepo.UpdateBasketAsync(basket);

            return basket;

        }

        public async Task<Core.Entities.OrderAggregate.Order> UpdatePaymentIntentToSucceededOrFailed(string paymentIntentId, bool isSucceeded)
        {
            var spec = new OrderWithPaymentIntentSpecififcations(paymentIntentId);

            var order = await _unitOfWork.Repository<Core.Entities.OrderAggregate.Order>().GetWithSpecAsync(spec);

            if(isSucceeded)
            {
                order.Status = OrderStatus.PaymentReceived;
            }
            else
            {
                order.Status = OrderStatus.PaymentFailed;
            }

            _unitOfWork.Repository<Core.Entities.OrderAggregate.Order>().Update(order);

            await _unitOfWork.CompleteAsync();

            return order;

        }
    }
}
