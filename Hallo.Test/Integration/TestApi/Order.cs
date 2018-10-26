using System;

namespace Hallo.Test.Integration.TestApi
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime Placed { get; set; }
    }

    public class ShippingInformation
    {
        public string RecipientName { get; set; }
        public string DeliveryAddress { get; set; }
    }
}