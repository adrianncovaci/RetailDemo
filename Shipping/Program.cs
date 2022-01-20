using Shipping.Messages;
using NServiceBus;

namespace Shipping
{
    public class Shipping
    {
        public static async Task Main(string[] args)
        {
            Console.Title = "Shipping";

            //configure logging first

            var endpointConfiguration = new NServiceBus.EndpointConfiguration("Shipping");

            var transport = endpointConfiguration.UseTransport<LearningTransport>();

            var routing = transport.Routing();

            routing.RouteToEndpoint(typeof(ShipWithMapple), "Shipping");
            routing.RouteToEndpoint(typeof(ShipWithAlpine), "Shipping");
            routing.RouteToEndpoint(typeof(ShipOrder), "Shipping");

            var persistence = endpointConfiguration.UsePersistence<LearningPersistence>();

            var endpointInstance = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);

            Console.WriteLine("Press Enter to exit.");
            Console.ReadLine();

            await endpointInstance.Stop()
                .ConfigureAwait(false);
        }
    }
}
