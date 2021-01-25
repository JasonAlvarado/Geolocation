using CommunicationHelper.Http;
using Geolocator.Service.RemoteModel;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Geolocator.Service
{
    class Program
    {
        private const string HostName = "rabbit-mq";
        private const string QueueName = "Geolocate";

        public static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = HostName };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            // Always ready to receive rabbitMq messages
            while (true)
            {
                channel.QueueDeclare(queue: QueueName,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += async (model, ea) =>
                {
                    await GeolocateRequest(ea);
                };

                channel.BasicConsume(queue: QueueName,
                                     autoAck: true,
                                     consumer: consumer);
            }
        }

        private static async Task GeolocateRequest(BasicDeliverEventArgs ea)
        {
            var message = Encoding.UTF8.GetString(ea.Body.ToArray());
            var geoRequest = JsonConvert.DeserializeObject<GeoRequestRemote>(message);

            HttpHelper httpClientHelper = new HttpHelper();
            var url = $"https://nominatim.openstreetmap.org/search?street={geoRequest.Street}%20{geoRequest.Number}&city={geoRequest.City}&country={geoRequest.Country}&format=json";
            var geolocateResponse = await httpClientHelper.Get(url);

            if (geolocateResponse.IsSuccessStatusCode)
            {
                try
                {
                    string content = await geolocateResponse.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<List<GeoResultRemote>>(content).FirstOrDefault();
                    result.GeoRequestId = geoRequest.Id;
                    result.State = "Terminado";

                    await httpClientHelper.Put("http://apigeo.service/api/search", result);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex?.InnerException?.Message);
                }
            }
        }
    }
}
