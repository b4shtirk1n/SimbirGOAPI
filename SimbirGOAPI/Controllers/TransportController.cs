using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimbirGOAPI.Attributes;
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
        [HttpGet("{" + nameof(id) + "}")]
        public async Task<ActionResult<User>> GetById(int id)
        {
            User user = new();

            return Ok(user);
        }
    }
}
