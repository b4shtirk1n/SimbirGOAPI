using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
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
        public async Task<IActionResult> GetById(long rentId)
        {
            return Ok();
        }

        [HttpGet(nameof(MyHistory))]
        public async Task<ActionResult<List<Rent>>> MyHistory()
        {
            return Ok();
        }

        [HttpGet($"{nameof(TransportHistory)}/{{{nameof(transportId)}}}")]
        public async Task<ActionResult<List<Rent>>> TransportHistory(long transportId)
        {
            return Ok();
        }

        [HttpPost($"{nameof(New)}/{{{nameof(transportId)}}}")]
        public async Task<IActionResult> New(long transportId, string rentType)
        {
            return Ok();
        }

        [HttpPost($"{nameof(End)}/{{{nameof(rentId)}}}")]
        public async Task<IActionResult> End(long rentId, string rentType)
        {
            return Ok();
        }
    }
}
