using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SimbirGOAPI.Attributes;
using SimbirGOAPI.Extensions;
using SimbirGOAPI.Models;

namespace SimbirGOAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [ServiceFilter(typeof(DbConnectionAttribute))]
    [ServiceFilter(typeof(CheckBlackListAttribute))]
    public class TransportController : ControllerBase
    {
        private readonly ILogger<TransportController> logger;
        private readonly SimbirGODbContext context;
        private readonly IMemoryCache cache;

        public TransportController(ILogger<TransportController> logger, SimbirGODbContext context,
            IMemoryCache cache)
        {
            this.logger = logger;
            this.context = context;
            this.cache = cache;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Transport>> GetById(long id)
        {
            if (await context.Transports.FirstOrDefaultAsync(c => c.Id == id)
                is not Transport transport)
                return BadRequest("This transport doesn't exist");

            return Ok(transport);
        }

        [HttpPost]
        public async Task<IActionResult> Create(TransportDTO transport)
        {
            if (transport.Type.GetEnumValue<TransportEnum>() is not TransportEnum type)
                return BadRequest("This transport type doesn't exist");

            if (transport.Color.GetEnumValue<ColorEnum>() is not ColorEnum color)
                return BadRequest("This color doesn't exist");

            await context.Transports.AddAsync(new Transport
            {
                Owner = int.Parse(User.GetClaimValue(nameof(Models.User.Id))),
                CanRented = transport.CanBeRented,
                Type = (int)type,
                Model = transport.Model,
                Color = (int)color,
                Identifier = transport.Identifier,
                Description = transport.Description,
                Latitude = (decimal)transport.Latitude,
                Longitude = (decimal)transport.Longitude,
                MinutePrice = transport.MinutePrice,
                DayPrice = transport.DayPrice
            });
            await context.SaveChangesAsync();
            logger.LogInformation($"Transport has been added");

            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, TransportDTO transport)
        {
            if (transport.Color.GetEnumValue<ColorEnum>() is not ColorEnum color)
                return BadRequest("This color doesn't exist");

            if (await context.Transports.FirstOrDefaultAsync(t => t.Id == id && t.User == long
                .Parse(User.GetClaimValue(nameof(Models.User.Id)))) == null)
                return BadRequest("Transport doesn't exist or the user is not the owner");

            var updateTransport = await context.Transports.FirstAsync(u => u.Id == long
                .Parse(User.GetClaimValue(nameof(Models.User.Id))));

            updateTransport.CanRented = transport.CanBeRented;
            updateTransport.Model = transport.Model;
            updateTransport.Color = (int)color;
            updateTransport.Identifier = transport.Identifier;
            updateTransport.Description = transport.Description;
            updateTransport.Latitude = (decimal)transport.Latitude;
            updateTransport.Longitude = (decimal)transport.Longitude;
            updateTransport.MinutePrice = transport.MinutePrice;
            updateTransport.DayPrice = transport.DayPrice;

            await context.SaveChangesAsync();
            logger.LogInformation($"Transport has been edited");

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Transport>> Delete(long id)
        {
            if (await context.Transports.FirstOrDefaultAsync(t => t.Id == id && t.User == long
                .Parse(User.GetClaimValue(nameof(Models.User.Id)))) == null)
                return BadRequest("Transport doesn't exist or the user is not the owner");

            return Ok();
        }
    }
}
