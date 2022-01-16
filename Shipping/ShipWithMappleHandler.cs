using NServiceBus;
using NServiceBus.Logging;
using Shipping.Messages;

namespace Shipping
{
    public class ShipWithMappleHandler : IHandleMessages<ShipWithMapple>
    {
        static ILog log = LogManager.GetLogger<ShipWithMapple>();
        const int maxTimelapse = 30;
        static Random random = new Random();
        public async Task Handle(ShipWithMapple message, IMessageHandlerContext context)
        {
            var waitingTime = random.Next(maxTimelapse);
            log.Info($"Shipping with mapple: Delaying for {waitingTime} seconds");
            await Task.Delay(waitingTime * 1000);
            await context.Reply(new ShipmentAcceptedByMapple());
        }
    }
}
