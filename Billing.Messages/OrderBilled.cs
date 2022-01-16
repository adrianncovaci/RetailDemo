namespace Billing.Messages
{
    public class OrderBilled: NServiceBus.IEvent
    {
        public string OrderId { get; set; }
    }
}
