using NServiceBus;

namespace Shipping.Messages
{
    public class ShipOrder: ICommand
    {
        public string OrderId { get; set; }
    }
}
