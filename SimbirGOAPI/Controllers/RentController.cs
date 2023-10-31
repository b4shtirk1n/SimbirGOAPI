using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SimbirGOAPI.Attributes;
using SimbirGOAPI.Extensions;
using SimbirGOAPI.Models;
using System.Collections.Generic;
using System.Security.Cryptography.Xml;

namespace SimbirGOAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [ServiceFilter(typeof(DbConnectionAttribute))]
    [ServiceFilter(typeof(CheckBlackListAttribute))]
    public class RentController : ControllerBase
    {
        private readonly ILogger<RentController> logger;
        private readonly SimbirGODbContext context;
        private readonly IMemoryCache cache;

        public RentController(ILogger<RentController> logger, SimbirGODbContext context,
            IMemoryCache cache)
        {
            this.logger = logger;
            this.context = context;
            this.cache = cache;
        }

        private long UserId => long.Parse(User.GetClaimValue(nameof(Rent.Id)));

        [HttpGet(nameof(Transport))]
        public async Task<ActionResult<List<Transport>>> Transport(decimal lat, decimal @long,
            decimal radius, string type)
        {
            if (type.GetEnumValue<TransportEnum>() is not TransportEnum transportType)
                return BadRequest("This transport type doesn't exist");

            if (await context.Transports.Where(t => t.Latitude >= lat
                && t.Latitude <= lat + radius && t.Longitude >= @long&& t.Longitude <= @long + radius
                && t.Type == (long)transportType).ToListAsync() is not List<Transport> rentedTransport)
                return BadRequest();

            return Ok(rentedTransport);
        }

        [HttpGet($"{{{nameof(rentId)}}}")]
        public async Task<ActionResult<Rent>> GetById(long rentId)
        {
            if (await context.Rents.Include(o => o.UserNavigation).Include(o => o.TypeNavigation)
                .Include(o => o.TransportNavigation).FirstOrDefaultAsync(r => r.Id == rentId
                && (r.User == UserId || r.TransportNavigation.Owner == UserId)) is not Rent rent)
                return BadRequest("Rent doesn't exist or the user is not the owner");

            return Ok(rent);
        }

        [HttpGet(nameof(MyHistory))]
        public async Task<ActionResult<List<Rent>>> MyHistory()
        {
            if (await context.Rents.Include(o => o.UserNavigation).Include(o => o.TypeNavigation)
                .Include(o => o.TransportNavigation).Where(r => r.User == UserId).ToListAsync()
                is not List<Rent> rents)
                return BadRequest("Rent doesn't exist or the user is not the owner");

            return Ok(rents);
        }

        [HttpGet($"{nameof(TransportHistory)}/{{{nameof(transportId)}}}")]
        public async Task<ActionResult<List<Rent>>> TransportHistory(long transportId)
        {
            if (await context.Rents.Include(o => o.UserNavigation).Include(o => o.TypeNavigation)
                .Include(o => o.TransportNavigation).Where(r => r.TransportNavigation.Owner == UserId
                && r.Transport == transportId).ToListAsync() is not List<Rent> rents)
                return BadRequest("Rent doesn't exist or the user is not the owner");

            return Ok(rents);
        }

        [HttpPost($"{nameof(New)}/{{{nameof(transportId)}}}")]
        public async Task<IActionResult> New(long transportId, string rentType)
        {
            if (rentType.GetEnumValue<RentEnum>() is not RentEnum type)
                return BadRequest("This color doesn't exist");

            Transport? transport = await context.Transports.FirstOrDefaultAsync(r => r.Owner != UserId);

            if (transport == null && await context.Rents.Include(o => o.TransportNavigation).FirstOrDefaultAsync(r
                => r.Transport == transportId && r.Type == (int)type) == null)
                return BadRequest();

            await context.Rents.AddAsync(new Rent
            {
                Transport = transportId,
                User = UserId,
                Type = (int)type,
                TimeStart = DateTime.Now
            });
            transport!.CanRented = false;
            await context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost($"{nameof(End)}/{{{nameof(rentId)}}}")]
        public async Task<IActionResult> End(long rentId, decimal lat, decimal @long)
        {
            if (await context.Rents.Include(o => o.TransportNavigation).FirstOrDefaultAsync(r
                => r.User == UserId) is not Rent rent)
                return BadRequest();

            rent.TransportNavigation.CanRented = true;
            rent.TransportNavigation.Latitude = lat;
            rent.TransportNavigation.Latitude = @long;
            rent.TimeEnd = DateTime.Now;
            await context.SaveChangesAsync();

            return Ok();
        }
    }
}
