using NServiceBus;
using NServiceBus.Logging;
using Shipping.Messages;

namespace Shipping
{
    public class ShipWithAlpineHandler : IHandleMessages<ShipWithAlpine>
    {
        static ILog log = LogManager.GetLogger<ShipWithAlpineHandler>();
        static int MaxDelay = 30;
        static Random random = new Random();

        public async Task Handle(ShipWithAlpine message, IMessageHandlerContext context)
        {
            var delaySeconds = random.Next(MaxDelay) * 1000;
            log.Info($"Shipping with alpine: delay {delaySeconds}");
            await Task.Delay(delaySeconds);
            await context.Reply(new ShipmentAcceptedByAlpine());

        }
    }
}
