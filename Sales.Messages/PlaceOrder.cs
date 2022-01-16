namespace Messages
{
    public class PlaceOrder: NServiceBus.ICommand
    {
        public string OrderId { get; set; }
    }
}
