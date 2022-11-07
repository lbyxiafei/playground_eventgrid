using Microsoft.Extensions.Hosting;
using Azure.Identity;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using System.Text;

namespace EventGridQueue
{
    public class QueueListener : BackgroundService
    {
        private readonly TimeSpan _period = TimeSpan.FromSeconds(3);
        private readonly QueueClient _queueClient;

        private readonly string _queueName = "universaleditorlogs";

        public QueueListener()
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
                    Console.WriteLine("Queue listener say hi!");
                    await DequeueMessagesAsync().ConfigureAwait(false);
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Queue listener error:" + ex.Message);
                }
            }
        }

        public async Task DequeueMessagesAsync(string queueName= "universaleditorlogs")
        {
            if (await _queueClient.ExistsAsync().ConfigureAwait(false))
            {
                // Receive and process 20 messages
                QueueMessage[] receivedMessages = await _queueClient.ReceiveMessagesAsync(20, TimeSpan.FromMinutes(5)).ConfigureAwait(false);

                foreach (QueueMessage message in receivedMessages)
                {
                    // Process (i.e. print) the messages in less than 5 minutes
                    var msgData = Encoding.UTF8.GetString(Convert.FromBase64String(message.Body.ToString()));
                    Console.WriteLine($"De-queued message: '{msgData}'");

                    // Delete the message
                    await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt).ConfigureAwait(false);
                }
            }
        }
    }
}
