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
        private readonly HttpClient httpClient;
        private const string HostName = "my-rabbit";//rabbit-mq
        private const string QueueName = "GeolocateRequest";

        public Program(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        static async Task Main()
        {
            GeoRequestRemote geoRequest = new GeoRequestRemote();
            //var factory = new ConnectionFactory() { HostName = HostName };
            //using (var connection = factory.CreateConnection())
            //{
            //    using (var channel = connection.CreateModel())
            //    {
            //        channel.QueueDeclare(queue: QueueName,
            //                             durable: false,
            //                             exclusive: false,
            //                             autoDelete: false,
            //                             arguments: null);

            //        var consumer = new EventingBasicConsumer(channel);
            //        consumer.Received += (model, ea) =>
            //        {
            //            var body = ea.Body.ToArray();
            //            var message = Encoding.UTF8.GetString(body);
            //            geoRequest = JsonConvert.DeserializeObject<GeoRequestRemote>(message);
            //            var result = Geolocate(geoRequest);
            //            SaveResult(result);
            //        };
            //        channel.BasicConsume(queue: QueueName,
            //                             autoAck: true,
            //                             consumer: consumer);
            //    }
            //}
        }

        private static async void SaveResult(Task<GeoResultRemote> result)
        {
            var httpClient = new HttpClient();
            #region HttpHeaders
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:19.0) Gecko/20100101 Firefox/19.0");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Charset", "ISO-8859-1");
            #endregion


            var httpContent = new StringContent(JsonConvert.SerializeObject(result), Encoding.UTF8, "application/json");
            var httpRequest = await httpClient.PutAsync("http://localhost:5000/api/search", httpContent);

            //if (httpRequest.IsSuccessStatusCode)
            //{
            //}
        }

        private static async Task<GeoResultRemote> Geolocate(GeoRequestRemote geoRequest)
        {
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
                GeoResultRemote geoResult = JsonConvert.DeserializeObject<GeoResultRemote>(content);
                return geoResult;
            }

            return null;
        }
    }
}
