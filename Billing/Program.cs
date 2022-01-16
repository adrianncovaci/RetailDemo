using NServiceBus;

namespace Billing
{
    public class Billing
    {
        static async Task Main()
        {
            Console.Title = "Billing";

            var endpointConfiguration = new NServiceBus.EndpointConfiguration("Billing");

            var transport = endpointConfiguration.UseTransport<LearningTransport>();

            var endpointInstance = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);

            
            Console.WriteLine("Press Enter to exit.");
            Console.ReadLine();

            await endpointInstance.Stop()
                .ConfigureAwait(false);
        }
    }
}
