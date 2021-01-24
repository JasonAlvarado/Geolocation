using Geolocator.Service.RemoteModel;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Geolocator.Service
{
    class Program
    {
        static async Task Main()
        {
            GeoRequestRemote geoRequest = new GeoRequestRemote();
            var factory = new ConnectionFactory() { HostName = "rabbit-mq" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "GeolocateRequest",
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        geoRequest = JsonConvert.DeserializeObject<GeoRequestRemote>(message);
                    };
                    channel.BasicConsume(queue: "GeolocateRequest",
                                         autoAck: true,
                                         consumer: consumer);

                    var httpClient = new HttpClient();

                    #region HttpHeaders
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml");
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:19.0) Gecko/20100101 Firefox/19.0");
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Charset", "ISO-8859-1");
                    #endregion

                    var url = $"https://nominatim.openstreetmap.org/search?street={geoRequest.Street}%20{geoRequest.Number}&city={geoRequest.City}&country={geoRequest.Country}&format=json";
                    var response = await httpClient.GetAsync(new Uri(url));

                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        var body = Encoding.UTF8.GetBytes(content);

                        channel.QueueDeclare(queue: "GeolocateResult",
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                        channel.BasicPublish(exchange: "",
                                             routingKey: "GeolocateResult",
                                             basicProperties: null,
                                             body: body);
                    }
                }
            }
        }
    }
}
