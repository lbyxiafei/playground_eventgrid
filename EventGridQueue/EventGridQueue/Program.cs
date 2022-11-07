using EventGridQueue;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

class EventGridQueueMain
{
    public static void Main(string[] args)
    {
        Console.WriteLine("hello");
        var host = new HostBuilder()
              .ConfigureHostConfiguration(configHost =>
              {
              })
              .ConfigureServices((hostContext, services) =>
              {
                  services.AddHostedService<QueueListener>();
                  services.AddHostedService<TestWorker>();

              })
             .UseConsoleLifetime()
             .Build();
        host.Run();
    }
}

