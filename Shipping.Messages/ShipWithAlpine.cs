using NServiceBus;

namespace Shipping.Messages
{
    public class ShipWithAlpine: ICommand
    {
        public string OrderId { get; set; }
    }
}
