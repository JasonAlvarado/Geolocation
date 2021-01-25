﻿using ApiGeo.Service.Model;
using ApiGeo.Service.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiGeo.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly DataContext context;

        public SearchController(DataContext context)
        {
            this.context = context;
        }

        [HttpPost("geolocalizar")]
        public async Task<IActionResult> Create(GeoRequest request)
        {
            await context.GeoRequests.AddAsync(request);
            await context.SaveChangesAsync();

            await context.GeoResults.AddAsync(new GeoResult() { GeoResultId = request.Id, State = "Procesando" });
            await context.SaveChangesAsync();

            SendMessage(request);
            return Ok(request.Id);
        }

        [HttpGet("geocodificar/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var geoRequest = await context.GeoResults.FirstOrDefaultAsync(x => x.GeoResultId == id);

            if (geoRequest != null)
                return Ok(geoRequest);

            return NotFound();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateRequestState(GeoResult geoResult)
        {
            var entity = await context.GeoResults.FirstOrDefaultAsync(x => x.GeoResultId == geoResult.GeoResultId);

            if (entity == null)
                return NotFound();

            entity.Lat = geoResult.Lat;
            entity.Lon = geoResult.Lon;
            entity.State = geoResult.State;
            //entity = geoResult;
            context.Entry(entity).State = EntityState.Modified;
            try
            {
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                var ex = e.InnerException.Message;
            }
            return Ok();
        }

        private void SendMessage(GeoRequest request)
        {
            const string hostName = "rabbit-mq";
            const string queuename = "Geolocate";
            var factory = new ConnectionFactory() { HostName = hostName };

            using var connection = factory.CreateConnection();
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: queuename,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                string message = JsonConvert.SerializeObject(request);
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: string.Empty,
                                     routingKey: queuename,
                                     basicProperties: null,
                                     body: body);
            }
        }
    }
}
