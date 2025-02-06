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

namespace Talabat.Service
{
    public class OrderService : IOrderService
    {
        private readonly IBasketRepository _basketRepo;
        private readonly IPaymentService _paymentService;
        private readonly IUnitOfWork _unitOfWork;


        public OrderService(
            IBasketRepository basketRepo,
            IPaymentService paymentService,
            IUnitOfWork unitOfWork)
        {
            _basketRepo = basketRepo;
            _paymentService = paymentService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Order?> CreateOrderAsync(string buyerEmail, string basketId, int deliveryMethodId, Address shippingAddress)
        {
            var basket = await _basketRepo.GetBasketAsync(basketId);

            var orderItems = new List<OrderItem>();

            if (basket?.Items?.Count > 0)
            {
                foreach(var item in basket.Items)
                {
                    var product = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);

                    var productItemOrdered = new ProductItemOrdered(item.Id, product.Name, product.PictureUrl);

                    var orderItem = new OrderItem(productItemOrdered,product.Price,item.Quantity);

                    orderItems.Add(orderItem);
                }
            }

            var subtotal = orderItems.Sum(orderItem => orderItem.Price * orderItem.Quantity);

            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(deliveryMethodId);

            var orderSpec = new OrderWithPaymentIntentSpecififcations(basket.PaymentIntentId);

            var existingOrder = await _unitOfWork.Repository<Order>().GetWithSpecAsync(orderSpec);

            if (existingOrder != null)
            {
                _unitOfWork.Repository<Order>().Delete(existingOrder);

                await _paymentService.CreateOrUpdatePaymentIntent(basketId);
            }


            var order = new Order(buyerEmail,shippingAddress, deliveryMethod, orderItems, subtotal, basket.PaymentIntentId);

            await _unitOfWork.Repository<Order>().AddAsync(order);

            var result = await _unitOfWork.CompleteAsync();

            if (result <= 0) return null;

            return order;
        }

        public async Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail)
        {
            var ordersRepo = _unitOfWork.Repository<Order>();

            var spec = new OrderSpecifications(buyerEmail);

            var orders = await ordersRepo.GetAllWithSpecAsync(spec);

            return orders;
        }

        public Task<Order?> GetOrderByIdForUserAsync(int orderId, string buyerEmail)
        {
            var orderRepo = _unitOfWork.Repository<Order>();

            var orderSpec = new OrderSpecifications(orderId,buyerEmail);

            var order = orderRepo.GetWithSpecAsync(orderSpec);

            return order;
        }

        public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
        {
            var deliveryMethodsRepo = _unitOfWork.Repository<DeliveryMethod>();

            var deliveryMethods = await deliveryMethodsRepo.GetAllAsync();

            return deliveryMethods;

        }
    }
}
