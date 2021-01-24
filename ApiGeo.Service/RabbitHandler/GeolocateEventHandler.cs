using ApiGeo.Service.Model;
using ApiGeo.Service.Persistence;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiGeo.Service.RabbitHandler
{
    public class GeolocateEventHandler
    {
        private readonly DataContext context;

        public GeolocateEventHandler(DataContext context)
        {
            this.context = context;
        }

        public async Task SaveGeolocateResult()
        {
            GeoResult geoRequest = new GeoResult();

            var factory = new ConnectionFactory() { HostName = "rabbit-mq" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "Geolocate",
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        geoRequest = JsonConvert.DeserializeObject<GeoResult>(message);
                    };

                    channel.BasicConsume(queue: "Geolocate",
                                         autoAck: true,
                                         consumer: consumer);

                    var entity = await context.GeoResults.FirstOrDefaultAsync(x => x.GeoResultId == geoRequest.GeoResultId);
                    entity.State = "Terminado";
                    context.Entry(entity).State = EntityState.Modified;
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
