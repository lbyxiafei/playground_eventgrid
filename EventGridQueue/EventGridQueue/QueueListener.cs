using Microsoft.Extensions.Hosting;

namespace EventGridQueue
{
    public class QueueListener : BackgroundService
    {
        private readonly TimeSpan _period = TimeSpan.FromSeconds(5);

        public QueueListener()
        {
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using PeriodicTimer timer = new PeriodicTimer(_period);

            while (!stoppingToken.IsCancellationRequested
                && await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    Console.WriteLine("Queue listener say hi!");
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Queue listener error!");
                }
            }
        }
    }
}
