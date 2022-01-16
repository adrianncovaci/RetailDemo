using NServiceBus;

namespace Shipping.Messages
{
    public class ShippingFailed: IEvent
    {
        public string OrderId { get; set; }
    }
}
