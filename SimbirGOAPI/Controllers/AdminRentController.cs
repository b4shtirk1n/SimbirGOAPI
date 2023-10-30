using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using SimbirGOAPI.Attributes;
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
            return Ok();
        }

        [HttpGet($"{nameof(UserHistory)}/{{{nameof(userId)}}}")]
        public async Task<ActionResult<List<Rent>>> UserHistory(long userId)
        {
            return Ok();
        }

        [HttpGet($"{nameof(TransportHistory)}/{{{nameof(transportId)}}}")]
        public async Task<ActionResult<List<Rent>>> TransportHistory(long transportId)
        {
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Create(RentDTO rent)
        {
            return Ok();
        }

        [HttpPost($"{nameof(End)}/{{{nameof(rentId)}}}")]
        public async Task<IActionResult> End(long rentId, decimal lat, decimal @long)
        {
            return Ok();
        }

        [HttpPut($"{{{nameof(id)}}}")]
        public async Task<IActionResult> Update(long id)
        {
            return Ok();
        }

        [HttpDelete($"{{{nameof(rentId)}}}")]
        public async Task<IActionResult> Delete(long rentId)
        {
            return Ok();
        }
    }
}
