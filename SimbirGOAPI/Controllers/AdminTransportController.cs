using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SimbirGOAPI.Attributes;
using SimbirGOAPI.Extensions;
using SimbirGOAPI.Models;

namespace SimbirGOAPI.Controllers
{
    [ApiController]
    [Route("api/Admin/Transport")]
    [Authorize(Roles = nameof(RoleEnum.Admin))]
    [ServiceFilter(typeof(DbConnectionAttribute))]
    [ServiceFilter(typeof(CheckBlackListAttribute))]
    public class AdminTransportController : ControllerBase
    {
        private readonly ILogger<AdminTransportController> logger;
        private readonly SimbirGODbContext context;
        private readonly IMemoryCache cache;

        public AdminTransportController(ILogger<AdminTransportController> logger,
            SimbirGODbContext context, IMemoryCache cache)
        {
            this.logger = logger;
            this.context = context;
            this.cache = cache;
        }

        [HttpGet]
        public async Task<ActionResult<List<Transport>>> GetBetween(int start, int end)
        {
            if (await context.Transports.Include(o => o.OwnerNavigation).Include(o => o.TypeNavigation)
                .Where(r => r.Id == start).Take(end).ToListAsync() is not List<Transport> transports)
                return BadRequest();

            return Ok(transports);
        }

        [HttpGet(nameof(id))]
        public async Task<ActionResult<Transport>> GetById(long id)
        {
            if (await context.Transports.Include(o => o.OwnerNavigation).Include(o => o.TypeNavigation)
                .FirstOrDefaultAsync(r => r.Id == id) is not Transport transport)
                return BadRequest();

            return Ok(transport);
        }

        [HttpPost]
        public async Task<IActionResult> Create(TransportAdminDTO transport)
        {
            if (transport.Type.GetEnumValue<TransportEnum>() is not TransportEnum type)
                return BadRequest("This transport type doesn't exist");

            if (transport.Color.GetEnumValue<ColorEnum>() is not ColorEnum color)
                return BadRequest("This color doesn't exist");

            if (await context.Users.FindAsync(transport.Owner) == null)
                return BadRequest();

            await context.Transports.AddAsync(new Transport
            {
                Owner = transport.Owner,
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

        [HttpPut(nameof(id))]
        public async Task<IActionResult> Update(long id, TransportAdminDTO transport)
        {
            if (transport.Type.GetEnumValue<TransportEnum>() is not TransportEnum type)
                return BadRequest("This transport type doesn't exist");

            if (transport.Color.GetEnumValue<ColorEnum>() is not ColorEnum color)
                return BadRequest("This color doesn't exist");

            if (await context.Users.FindAsync(transport.Owner) == null)
                return BadRequest();

            await context.Transports.AddAsync(new Transport
            {
                Owner = transport.Owner,
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

        [HttpDelete(nameof(id))]
        public async Task<IActionResult> Delete(long id)
        {
            if (await context.Transports.FindAsync(id) is not Transport transport)
                return BadRequest(Error.USER_DOESNT_EXIST);

            context.Transports.Remove(transport);
            await context.SaveChangesAsync();

            return Ok();
        }
    }
}
