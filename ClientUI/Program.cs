using NServiceBus;

class Program 
{

    static NServiceBus.Logging.ILog log = NServiceBus.Logging.LogManager.GetLogger<Program>();

    static async Task RunLoop(IEndpointInstance endpointInstance)
    {
        string lastOrder = String.Empty;
        while (true)
        {
            log.Info("Press 'P' to place an order, or 'Q' to quit.");
            var key = Console.ReadKey();
            Console.WriteLine();


            switch (key.Key)
            {
                case ConsoleKey.P:
                    // Instantiate the command
                    var command = new Messages.PlaceOrder
                    {
                        OrderId = Guid.NewGuid().ToString()
                    };
                    lastOrder = command.OrderId;

                    // Send the command to the local endpoint
                    log.Info($"Sending PlaceOrder command, OrderId = {command.OrderId}");
                    await endpointInstance.Send(command)
                        .ConfigureAwait(false);


                    break;


                case ConsoleKey.C:
                    var cancelCommand = new Messages.CancelOrder
                    {
                        OrderId = lastOrder
                    };

                    await endpointInstance.Send(cancelCommand)
                        .ConfigureAwait(false);
                    log.Info($"Sent a correlated message to {cancelCommand.OrderId}");
                    break;

                case ConsoleKey.Q:
                    return;

                default:
                    log.Info("Unknown input. Please try again.");
                    break;
            }
        }
    }

    static async Task Main()
    {
        
        Console.Title = "ClientUI";

        var endpointConfiguration = new NServiceBus.EndpointConfiguration("ClientUI");

        var transport = endpointConfiguration.UseTransport<LearningTransport>();

        var routing = transport.Routing();

        routing.RouteToEndpoint(typeof(Messages.PlaceOrder), "Sales");
        routing.RouteToEndpoint(typeof(Messages.CancelOrder), "Sales");

        var endpointInstance = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);

        await RunLoop(endpointInstance).ConfigureAwait(false);

        await endpointInstance.Stop().ConfigureAwait(false);
    }
}
