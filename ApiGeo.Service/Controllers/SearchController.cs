using ApiGeo.Service.Base;
using ApiGeo.Service.Model;
using ApiGeo.Service.Persistence;
using CommunicationHelper.RabbitMq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ApiGeo.Service.Controllers
{
    public class SearchController : ApiBaseController
    {
        private readonly DataContext context;
        private readonly IRabbitMqHelper rabbitMqHelper;

        public SearchController(DataContext context, IRabbitMqHelper rabbitMqHelper)
        {
            this.context = context;
            this.rabbitMqHelper = rabbitMqHelper;
        }

        [HttpPost("geolocalizar")]
        public async Task<IActionResult> Create(GeoRequest request)
        {
            await context.GeoRequests.AddAsync(request);
            await context.SaveChangesAsync();

            var geoResult = new GeoResult() { GeoRequestId = request.Id, State = "Procesando" };
            await context.GeoResults.AddAsync(geoResult);
            await context.SaveChangesAsync();

            await rabbitMqHelper.PublishMessage(request);
            return Ok(geoResult);
        }

        [HttpGet("geocodificar/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var geoRequest = await context.GeoResults.FirstOrDefaultAsync(x => x.GeoRequestId == id);

            if (geoRequest == null)
                return NotFound();

            return Ok(geoRequest);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateRequestState(GeoResult geoResult)
        {
            var entity = await context.GeoResults.FirstOrDefaultAsync(x => x.GeoRequestId == geoResult.GeoRequestId);

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
