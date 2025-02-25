﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.OrderAggregate;

namespace Talabat.Core.Specifications.OrderSpecs
{
    public class OrderWithPaymentIntentSpecififcations : BaseSpecifications<Order>
    {

        public OrderWithPaymentIntentSpecififcations(string paymentIntentId)
            : base(O => O.PaymentIntentId == paymentIntentId)
        {
            
        }

    }
}
