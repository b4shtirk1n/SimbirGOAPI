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

        private long UserId => int.Parse(User.GetClaimValue(nameof(Models.User.Id)));

        [HttpGet($"{{{nameof(id)}}}")]
        public async Task<ActionResult<Transport>> GetById(long id)
        {
            if (await context.Transports.Include(o => o.OwnerNavigation).Include(o => o.TypeNavigation)
                .FirstOrDefaultAsync(c => c.Id == id) is not Transport transport)
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
                Owner = UserId,
                CanRented = transport.CanBeRented,
                Type = (int)type,
                Model = transport.Model,
                Color = (int)color,
                Identifier = transport.Identifier,
                Description = transport.Description,
                Latitude = transport.Latitude,
                Longitude = transport.Longitude,
                MinutePrice = transport.MinutePrice,
                DayPrice = transport.DayPrice
            });
            await context.SaveChangesAsync();
            logger.LogInformation($"Transport has been added");

            return Ok();
        }

        [HttpPut($"{{{nameof(id)}}}")]
        public async Task<IActionResult> Update(long id, TransportDTO transport)
        {
            if (transport.Color.GetEnumValue<ColorEnum>() is not ColorEnum color)
                return BadRequest("This color doesn't exist");

            if (await context.Transports.FirstOrDefaultAsync(t => t.Id == id
                && t.Owner == UserId) == null)
                return BadRequest("Transport doesn't exist or the user is not the owner");

            Transport updateTransport = await context.Transports.FirstAsync(u => u.Id == UserId);

            updateTransport.CanRented = transport.CanBeRented;
            updateTransport.Model = transport.Model;
            updateTransport.Color = (int)color;
            updateTransport.Identifier = transport.Identifier;
            updateTransport.Description = transport.Description;
            updateTransport.Latitude = transport.Latitude;
            updateTransport.Longitude = transport.Longitude;
            updateTransport.MinutePrice = transport.MinutePrice;
            updateTransport.DayPrice = transport.DayPrice;

            await context.SaveChangesAsync();
            logger.LogInformation($"Transport has been edited");

            return Ok();
        }

        [HttpDelete($"{{{nameof(id)}}}")]
        public async Task<ActionResult<Transport>> Delete(long id)
        {
            if (await context.Transports.FirstOrDefaultAsync(t => t.Id == id && t.Owner == UserId)
                is not Transport transport)
                return BadRequest("Transport doesn't exist or the user is not the owner");

            context.Remove(transport);
            await context.SaveChangesAsync();

            return Ok();
        }
    }
}
