namespace Messges
{
    public class OrderPlaced: NServiceBus.IEvent
    {
        public string OrderId { get; set; }
    }
}
