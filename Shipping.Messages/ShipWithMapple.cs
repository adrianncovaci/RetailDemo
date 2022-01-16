namespace Shipping.Messages
{
    public class ShipWithMapple: NServiceBus.ICommand
    {
        public string OrderId { get; set; }
    }
}
