using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SimbirGOAPI.Attributes;
using SimbirGOAPI.Extensions;
using SimbirGOAPI.Models;

namespace SimbirGOAPI.Controllers
{
    [ApiController]
    [Route("api/Admin/Account/")]
    [Authorize(Roles = nameof(RoleEnum.Admin))]
    [ServiceFilter(typeof(DbConnectionAttribute))]
    [ServiceFilter(typeof(CheckBlackListAttribute))]
    public class AdminAccountController : ControllerBase
    {
        private readonly ILogger<AdminAccountController> logger;
        private readonly SimbirGODbContext context;
        private readonly IMemoryCache cache;

        public AdminAccountController(ILogger<AdminAccountController> logger,
            SimbirGODbContext context, IMemoryCache cache)
        {
            this.logger = logger;
            this.context = context;
            this.cache = cache;
        }

        [HttpGet]
        public async Task<ActionResult<List<User>>> GetBetween(int start, int count)
        {
            try
            {
                return Ok(await context.Users.Where(u => u.Id >= start).Take(count).ToListAsync());
            }
            finally
            {
                BadRequest(Error.USER_DOESNT_EXIST);
            }
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetById(long id)
        {
            string cacheKey = $"{nameof(User)}{id}";
            User? user = cache.Get(cacheKey) as User;

            if (user == null)
            {
                if ((user = await context.Users.FindAsync(id)) == null)
                    return BadRequest(Error.USER_DOESNT_EXIST);

                cache.Set(cacheKey, user);
                logger.LogInformation($"{cacheKey} add to cache");
            }
            else
            {
                logger.LogInformation($"{cacheKey} taken from cache");
            }
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserAdminDTO user)
        {
            if (await context.Users.FirstOrDefaultAsync(u => u.Username == user.Username) != null)
                return BadRequest(Error.USER_EXIST);

            await context.Users.AddAsync(new User
            {
                Username = user.Username,
                Password = user.Password,
                Role = user.IsAdmin ? (int)RoleEnum.Admin : (int)RoleEnum.Client,
                Balance = user.Balance
            });
            await context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, UserAdminDTO user)
        {
            if (await context.Users.FindAsync(id) is not User updateUser)
                return BadRequest(Error.USER_DOESNT_EXIST);

            updateUser.Username = user.Username;
            updateUser.Password = user.Password;
            updateUser.Role = user.IsAdmin ? (int)RoleEnum.Admin : (int)RoleEnum.Client;
            updateUser.Balance = user.Balance;

            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            if (await context.Users.FindAsync(id) is not User user)
                return BadRequest(Error.USER_DOESNT_EXIST);

            context.Remove(user);
            await context.SaveChangesAsync();

            return Ok();
        }
    }
}
