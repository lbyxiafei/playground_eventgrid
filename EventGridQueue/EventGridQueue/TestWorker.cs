using Azure.Identity;
using Azure.Storage.Queues;
using Microsoft.Extensions.Hosting;

namespace EventGridQueue
{
    public class TestWorker : BackgroundService
    {
        private readonly TimeSpan _period = TimeSpan.FromSeconds(3);
        private readonly QueueClient _queueClient;

        private readonly string _queueName = "universaleditorlogs";

        public TestWorker()
        {
            var queueEndpoint = $"https://ztpteststorage.queue.core.windows.net/{_queueName}";
            _queueClient = new QueueClient(
                new Uri(queueEndpoint),
                new DefaultAzureCredential());
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using PeriodicTimer timer = new PeriodicTimer(_period);

            while (!stoppingToken.IsCancellationRequested
                && await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    Console.WriteLine("Test worker say hi!");
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Test worker error:" + ex.Message);
                }
            }
        }
    }
}
