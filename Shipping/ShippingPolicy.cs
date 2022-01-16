using Messges;
using Billing.Messages;
using Shipping.Messages;
using NServiceBus;

namespace Shipping
{
    public class ShippingPolicy : Saga<ShippingPolicyData>, IAmStartedByMessages<Messges.OrderPlaced>, IAmStartedByMessages<OrderBilled>
    {
        static NServiceBus.Logging.ILog log = NServiceBus.Logging.LogManager.GetLogger<ShippingPolicy>();
        public Task Handle(OrderPlaced message, IMessageHandlerContext context)
        {
            log.Info($"Received order placed, OrderId = {message.OrderId}");
            Data.IsOrderPlaced = true;
            return ProcessOrder(context);
        }

        public Task Handle(OrderBilled message, IMessageHandlerContext context)
        {
            log.Info($"Received order billed, OrderId = {message.OrderId}");
            Data.IsOrderBilled = true;
            return ProcessOrder(context);
        }

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<ShippingPolicyData> mapper)
        {
            mapper.ConfigureMapping<OrderPlaced>(map => map.OrderId)
                .ToSaga(saga => saga.OrderId);

            mapper.ConfigureMapping<OrderBilled>(map => map.OrderId)
                .ToSaga(saga => saga.OrderId);
        }

        private async Task ProcessOrder(IMessageHandlerContext context)
        {
            if (Data.IsOrderBilled && Data.IsOrderPlaced)
            {
                await context.SendLocal(new ShipOrder() { OrderId = Data.OrderId });
                MarkAsComplete();
            }
        }
    }
}
