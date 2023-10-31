using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SimbirGOAPI.Attributes;
using SimbirGOAPI.Extensions;
using SimbirGOAPI.Models;

namespace SimbirGOAPI.Controllers
{
    [ApiController]
    [Route("api/Admin/Rent")]
    [Authorize(Roles = nameof(RoleEnum.Admin))]
    [ServiceFilter(typeof(DbConnectionAttribute))]
    [ServiceFilter(typeof(CheckBlackListAttribute))]
    public class AdminRentController : ControllerBase
    {
        private readonly ILogger<AdminRentController> logger;
        private readonly SimbirGODbContext context;
        private readonly IMemoryCache cache;

        public AdminRentController(ILogger<AdminRentController> logger,
            SimbirGODbContext context, IMemoryCache cache)
        {
            this.logger = logger;
            this.context = context;
            this.cache = cache;
        }

        [HttpGet(nameof(rentId))]
        public async Task<ActionResult<Rent>> GetById(long rentId)
        {
            if (await context.Rents.Include(o => o.UserNavigation).Include(o => o.TypeNavigation)
                .Include(o => o.TransportNavigation).FirstOrDefaultAsync(r => r.Id == rentId)
                is not Rent rent)
                return BadRequest();

            return Ok(rent);
        }

        [HttpGet($"{nameof(UserHistory)}/{{{nameof(userId)}}}")]
        public async Task<ActionResult<List<Rent>>> UserHistory(long userId)
        {
            if (await context.Rents.Include(o => o.UserNavigation).Include(o => o.TypeNavigation)
                .Include(o => o.TransportNavigation).Where(r => r.User == userId).ToListAsync()
                is not List<Rent> rents)
                return BadRequest();

            return Ok(rents);
        }

        [HttpGet($"{nameof(TransportHistory)}/{{{nameof(transportId)}}}")]
        public async Task<ActionResult<List<Rent>>> TransportHistory(long transportId)
        {
            if (await context.Rents.Include(o => o.UserNavigation).Include(o => o.TypeNavigation)
                .Include(o => o.TransportNavigation).Where(r => r.Transport == transportId).ToListAsync()
                is not List<Rent> rents)
                return BadRequest();

            return Ok(rents);
        }

        [HttpPost]
        public async Task<IActionResult> Create(RentDTO rent)
        {
            if (rent.Type.GetEnumValue<RentEnum>() is not RentEnum type)
                return BadRequest("This color doesn't exist");

            Transport? transport = await context.Transports.FindAsync(rent.Transport);

            if (transport == null && await context.Users.FindAsync(rent.User) == null)
                return BadRequest();

            await context.Rents.AddAsync(new Rent
            {
                Transport = rent.Transport,
                User = rent.User,
                TimeStart = rent.Start,
                TimeEnd = rent.End,
                Type = (int)type,
                Price = rent.Price
            });
            transport!.CanRented = false;

            if (transport!.Type == (int)RentEnum.Days)
                transport.DayPrice = rent.PriceOfUnit;
            else
                transport.MinutePrice = rent.PriceOfUnit;

            await context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost($"{nameof(End)}/{{{nameof(rentId)}}}")]
        public async Task<IActionResult> End(long rentId, decimal lat, decimal @long)
        {
            if (await context.Rents.Include(o => o.TransportNavigation).FirstOrDefaultAsync(r
                => r.Id == rentId) is not Rent rent)
                return BadRequest();

            rent.TransportNavigation.CanRented = true;
            rent.TransportNavigation.Latitude = lat;
            rent.TransportNavigation.Longitude = @long;
            rent.TimeEnd = DateTime.Now;
            await context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut($"{{{nameof(id)}}}")]
        public async Task<IActionResult> Update(long id, RentDTO rent)
        {
            if (rent.Type.GetEnumValue<RentEnum>() is not RentEnum type)
                return BadRequest("This color doesn't exist");

            Transport? transport = await context.Transports.FindAsync(rent.Transport);
            Rent? updateRent = await context.Rents.Include(o => o.TransportNavigation)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (transport == null && updateRent == null && await context.Users
                .FindAsync(rent.User) == null)
                return BadRequest();

            updateRent!.Transport = rent.Transport;
            updateRent.User = rent.User;
            updateRent.TimeStart = rent.Start;
            updateRent.TimeEnd = rent.End;
            updateRent.Type = (int)type;
            updateRent.Price = rent.Price;

            if (transport!.Type == (int)RentEnum.Days)
                transport.DayPrice = rent.PriceOfUnit;
            else
                transport.MinutePrice = rent.PriceOfUnit;

            await context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete($"{{{nameof(rentId)}}}")]
        public async Task<IActionResult> Delete(long rentId)
        {
            if (await context.Rents.FindAsync(rentId) is not Rent rent)
                return BadRequest();

            context.Rents.Remove(rent);
            await context.SaveChangesAsync();

            return Ok();
        }
    }
}
