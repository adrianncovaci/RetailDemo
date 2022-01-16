using NServiceBus;
using NServiceBus.Logging;
using Shipping.Messages;

namespace Shipping
{
    class ShipOrderWorkFlow: Saga<ShipOrderWorkFlow.ShipOrderData>,
        IAmStartedByMessages<ShipOrder>,
        IHandleMessages<ShipmentAcceptedByMapple>,
        IHandleMessages<ShipmentAcceptedByAlpine>,
        IHandleTimeouts<ShipOrderWorkFlow.ShippingEscalation>
    {
        static ILog log = LogManager.GetLogger<ShipOrderWorkFlow>();

        public async Task Handle(ShipOrder message, IMessageHandlerContext context)
        {
            log.Info($"ShipOrderWorkflow for Order #{Data.OrderId} - Trying Maple first.");

            // Execute order to ship with Maple
            await context.Send(new ShipWithMapple() { OrderId = message.OrderId });

            // Add timeout to escalate if Maple did not ship in time.
            await RequestTimeout(context, TimeSpan.FromSeconds(20), new ShippingEscalation());
        }

        public async Task Timeout(ShippingEscalation timeout, IMessageHandlerContext context)
        {
            if (!Data.ShipmentAcceptedByMapple)
            {
                if (!Data.ShipmentOrderSentToAlpine)
                {
                    log.Info($"Order [{Data.OrderId}] - We didn't receive answer from Maple, let's try Alpine.");
                    Data.ShipmentOrderSentToAlpine = true;
                    await context.Send(new ShipWithAlpine { OrderId = Data.OrderId });
                    await RequestTimeout(context, TimeSpan.FromSeconds(20), new ShippingEscalation());
                }
                else if (!Data.ShipmentAcceptedByAlpine)
                {
                    log.Info($"Order [{Data.OrderId}] - Shipment failed for both Alpine and Mapple.");
                    var failedEvent = new ShippingFailed
                    {
                        OrderId = Data.OrderId
                    };
                    await context.Publish(failedEvent);
                }
            }
        }

        public Task Handle(ShipmentAcceptedByMapple message, IMessageHandlerContext context)
        {
            if (!Data.ShipmentOrderSentToAlpine)
            {
                log.Info($"Shipment accepted by mapple");
                Data.ShipmentAcceptedByMapple = true;
                MarkAsComplete();
            }
            return Task.CompletedTask;
        }

        public Task Handle(ShipmentAcceptedByAlpine message, IMessageHandlerContext context)
        {
            if (!Data.ShipmentOrderSentToAlpine)
            {
                log.Info($"Shipment accepted by alpine");
                Data.ShipmentAcceptedByAlpine = true;
                MarkAsComplete();
            }
            return Task.CompletedTask;
        }

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<ShipOrderData> mapper)
        {
            mapper.ConfigureMapping<ShipOrder>(map => map.OrderId)
                .ToSaga(saga => saga.OrderId);
        }

        internal class ShipOrderData: ContainSagaData
        {
            public string OrderId { get; set; }
            public bool ShipmentAcceptedByMapple  { get; set; }
            public bool ShipmentOrderSentToAlpine  { get; set; }
            public bool ShipmentAcceptedByAlpine  { get; set; }
        }

        internal class ShippingEscalation
        {
        }
    }
}
