using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System; // Namespace for Console output
using System.Configuration; // Namespace for ConfigurationManager
using System.Threading.Tasks; // Namespace for Task
using Azure.Identity;
using Azure.Storage.Queues; // Namespace for Queue storage types
using Azure.Storage.Queues.Models; // Namespace for PeekedMessage
using System.Text;
using Newtonsoft.Json.Linq;
using Azure;
using System.Runtime.Serialization.Formatters.Binary;

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
                    //var data = Convert.ToBase64String(message.Body.ToArray());
                    var buffer = Convert.FromBase64String(message.Body.ToString());
                    Console.WriteLine(buffer);
                    var data = Encoding.UTF8.GetString(buffer);
                    Console.WriteLine(data);

                    //var buffer = message.Body.ToArray();

                    //string returnValue;

                    //using (MemoryStream ms = new MemoryStream(buffer))
                    //{
                    //    ms.Position = 0;
                    //    BinaryFormatter bf = new BinaryFormatter();
                    //    returnValue = (string)bf.Deserialize(ms);
                    //}

                    // Process (i.e. print) the messages in less than 5 minutes
                    Console.WriteLine($"De-queued message: '{data}'");

                    // Delete the message
                    await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt).ConfigureAwait(false);
                }
            }
        }
    }
}
