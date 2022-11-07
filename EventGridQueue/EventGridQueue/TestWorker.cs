using Microsoft.Extensions.Hosting;

namespace EventGridQueue
{
    public class TestWorker : BackgroundService
    {
        private readonly TimeSpan _period = TimeSpan.FromSeconds(5);

        public TestWorker()
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
                    Console.WriteLine("Test worker say hi!");
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Test worker error!");
                }
            }
        }
    }
}
