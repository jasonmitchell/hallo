using System.Collections.Generic;

namespace Hallo.Test.Integration.TestApi
{
    public class OrderRepresentation : Hal<Order>
    {
        protected override IEnumerable<Link> LinksFor(Order resource)
        {
            yield return new Link("self", $"/orders/{resource.Id}");
        }

        protected override object EmbeddedFor(Order resource)
        {
            return new
            {
                ShippingInformation = new ShippingInformation
                {
                    RecipientName = "Geoffrey Merrill",
                    DeliveryAddress = "The Place"
                }
            };
        }
    }
}