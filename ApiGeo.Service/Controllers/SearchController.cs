using ApiGeo.Service.Base;
using ApiGeo.Service.Model;
using ApiGeo.Service.Persistence;
using ApiGeo.Service.RabbitService;
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
    public class SearchController : ApiBaseController
    {
        private readonly DataContext context;
        private readonly IRabbitMqPublishMessage rabbitMqPublishMessage;

        public SearchController(DataContext context, IRabbitMqPublishMessage rabbitMqPublishMessage)
        {
            this.context = context;
            this.rabbitMqPublishMessage = rabbitMqPublishMessage;
        }

        [HttpPost("geolocalizar")]
        public async Task<IActionResult> Create(GeoRequest request)
        {
            await context.GeoRequests.AddAsync(request);
            await context.SaveChangesAsync();

            await context.GeoResults.AddAsync(new GeoResult() { GeoResultId = request.Id, State = "Procesando" });
            await context.SaveChangesAsync();

            await rabbitMqPublishMessage.PublishMessage(request);
            return Ok(request.Id);
        }

        [HttpGet("geocodificar/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var geoRequest = await context.GeoResults.FirstOrDefaultAsync(x => x.GeoResultId == id);

            if (geoRequest == null)
                return NotFound();

            return Ok(geoRequest);
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

            context.Entry(entity).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}
