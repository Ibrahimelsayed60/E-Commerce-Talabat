﻿using System.ComponentModel.DataAnnotations;
using Talabat.Core.Entities.OrderAggregate;

namespace Talabat.APIs.Dtos
{
    public class OrderDto
    {

        [Required]
        public string BasketId { get; set; }

        public int DeliveryMethodId { get; set; }

        public AddressDto shipToAddress { get; set; }
    }
}
