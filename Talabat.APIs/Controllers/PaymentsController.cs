using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Talabat.APIs.Errors;
using Talabat.Core.Entities;
using Talabat.Core.Entities.OrderAggregate;
using Talabat.Core.Services.Contract;

namespace Talabat.APIs.Controllers
{
    [Authorize]
    public class PaymentsController : BaseApiController
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentsController> _logger;
        private const string _whSecret = "whsec_39e3cee26e52cf3f3da8a6ba783e8469ac046c8e0412c056fd76dfd7ae8ad4c6";

        public PaymentsController(IPaymentService paymentService, ILogger<PaymentsController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }


        [ProducesResponseType(typeof(CustomerBasket), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [HttpPost("{basketId}")]
        public async Task<ActionResult<CustomerBasket>> CreateOrUpdatePaymentIntent(string basketId)
        {
            var basket = await _paymentService.CreateOrUpdatePaymentIntent(basketId);

            if (basket is null) return BadRequest(new ApiResponse(400, "An Error With Your Basket"));

            return Ok(basket);
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            var stripeEvent = EventUtility.ConstructEvent(json,
                    Request.Headers["Stripe-Signature"], _whSecret);

            var paymentIntent = (PaymentIntent)stripeEvent.Data.Object;

            Order order;

            // Handle the event
            switch (stripeEvent.Type)
            {
                case "payment_intent.succeeded":
                    order = await _paymentService.UpdatePaymentIntentToSucceededOrFailed(paymentIntent.Id, true);
                    _logger.LogInformation("Payment is Sussedded :)", paymentIntent.Id);
                    break;
                case "payment_intent.payment_failed":
                    order = await _paymentService.UpdatePaymentIntentToSucceededOrFailed(paymentIntent.Id, false);
                    _logger.LogInformation("Payment is Failed :( ", paymentIntent.Id);
                    break;
            }

            return Ok();
        }


    }
}
