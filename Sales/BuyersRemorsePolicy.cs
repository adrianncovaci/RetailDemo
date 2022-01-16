using Messages;
using Messges;
using NServiceBus;
using NServiceBus.Logging;

namespace Sales
{
    public class BuyersRemorsePolicy : Saga<BuyersRemorseState>,
        IAmStartedByMessages<PlaceOrder>,
        IHandleMessages<Messages.CancelOrder>,
        IHandleTimeouts<BuyersRemorseIsOver>
    {
        static ILog log = LogManager.GetLogger<BuyersRemorsePolicy>();

        public async Task Handle(PlaceOrder message, IMessageHandlerContext context)
        {
            log.Info($"Received place order {message.OrderId}");

            Data.OrderId = message.OrderId;

            log.Info($"Started cooldown period for order id = [{message.OrderId}]");

            await RequestTimeout(context, TimeSpan.FromSeconds(10), new BuyersRemorseIsOver());
        }

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<BuyersRemorseState> mapper)
        {
            mapper.ConfigureMapping<PlaceOrder>(map => map.OrderId)
                .ToSaga(saga => saga.OrderId);
            mapper.ConfigureMapping<CancelOrder>(map => map.OrderId)
                .ToSaga(saga => saga.OrderId);
        }

        public async Task Timeout(BuyersRemorseIsOver timeout, IMessageHandlerContext context)
        {
            log.Info($"Cooldown period for order id = [{Data.OrderId}] has elapsed.");
            
            var orderPlaced = new OrderPlaced
            {
                OrderId = Data.OrderId
            };

            await context.Publish(orderPlaced);

            MarkAsComplete();
        }

        public Task Handle(CancelOrder message, IMessageHandlerContext context)
        {
            log.Info($"Canceled order id = [{Data.OrderId}]"); 
            MarkAsComplete();
            return Task.CompletedTask;
        }

    }


    public class BuyersRemorseState: ContainSagaData
    {
        public string OrderId { get; set; }
    }

    public class BuyersRemorseIsOver
    {
    }
}

