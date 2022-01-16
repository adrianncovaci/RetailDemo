using Billing.Messages;
using Messges;
using NServiceBus;

namespace Billing
{
    public class OrderPlacedHandler : NServiceBus.IHandleMessages<Messges.OrderPlaced>
    {
        static NServiceBus.Logging.ILog log = NServiceBus.Logging.LogManager.GetLogger<OrderPlacedHandler>();
        public Task Handle(OrderPlaced message, IMessageHandlerContext context)
        {
            log.Info($"Received OrderPlaced, OrderId = {message.OrderId} - Charging credit card...");

            var orderBilled = new OrderBilled
            {
                OrderId = message.OrderId
            };

            log.Info($"Sending Order Billed {orderBilled.OrderId}") ;
            return context.Publish(orderBilled);
        }
    }
}
