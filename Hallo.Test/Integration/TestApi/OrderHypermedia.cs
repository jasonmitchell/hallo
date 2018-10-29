using System.Collections.Generic;

namespace Hallo.Test.Integration.TestApi
{
    public class OrderRepresentation : Hal<Order>,
                                       IHalLinks<Order>,
                                       IHalEmbedded<Order>
    {
        public IEnumerable<Link> LinksFor(Order resource)
        {
            yield return new Link("self", $"/orders/{resource.Id}");
        }

        public object EmbeddedFor(Order resource)
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