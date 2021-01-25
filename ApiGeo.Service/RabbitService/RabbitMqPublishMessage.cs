using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiGeo.Service.RabbitService
{
    public class RabbitMqPublishMessage : IRabbitMqPublishMessage
    {
        private const string hostName = "rabbit-mq";
        private const string queuename = "Geolocate";

        public Task PublishMessage<T>(T messageEntity) where T : class
        {
            var factory = new ConnectionFactory() { HostName = hostName };

            using var connection = factory.CreateConnection();
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: queuename,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                string message = JsonConvert.SerializeObject(messageEntity);
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: string.Empty,
                                     routingKey: queuename,
                                     basicProperties: null,
                                     body: body);
            }

            return Task.CompletedTask;
        }
    }
}
