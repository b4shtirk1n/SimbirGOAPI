using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using SimbirGOAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SimbirGOAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> logger;
        private readonly PostgresContext context;
        private readonly List<string> blackList;
        private readonly IMemoryCache cache;

        public AccountController(ILogger<AccountController> logger, PostgresContext context,
            List<string> blackList, IMemoryCache cache)
        {
            this.logger = logger;
            this.context = context;
            this.blackList = blackList;
            this.cache = cache;
        }

        [AllowAnonymous]
        [HttpPost(nameof(SingIn))]
        public async Task<ActionResult<string>> SingIn(UserDTO user)
        {
            if (!await context.Database.CanConnectAsync())
                return StatusCode(StatusCodes.Status503ServiceUnavailable,
                    Error.DB_CONNECTION_FAILED);

            if (await context.Users.FirstOrDefaultAsync(u => u.Username == user.Username
                && u.Password == HashPassword(user.Password)) is not User currentUser)
                return BadRequest("User not exist or uncorrected password");

            return Ok(GenerateToken(currentUser));
        }

        [AllowAnonymous]
        [HttpPost(nameof(SingUp))]
        public async Task<IActionResult> SingUp(UserDTO user)
        {
            if (!await context.Database.CanConnectAsync())
                return StatusCode(StatusCodes.Status503ServiceUnavailable,
                    Error.DB_CONNECTION_FAILED);

            if (await context.Users.FirstOrDefaultAsync(u => u.Username == user.Username) != null)
                return BadRequest("This user already exist");

            await context.Users.AddAsync(new User
            {
                Username = user.Username,
                Password = HashPassword(user.Password)
            });
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost(nameof(LogOut))]
        public IActionResult LogOut()
        {
            if (AuthOptions.IsTokenTerminate(cache, Request))
                return Unauthorized();

            string token = Request.Headers.Authorization;

            blackList.Add(token);
            logger.LogInformation($"{nameof(SecurityToken)}: {token} add to {nameof(blackList)}");

            cache.Set(nameof(SecurityToken), blackList, TimeSpan.FromMinutes(2));
            logger.LogInformation($"{nameof(blackList)} add to cache");

            return Ok();
        }

        [HttpPost(nameof(Update))]
        public async Task<IActionResult> Update(UserDTO user)
        {
            if (!await context.Database.CanConnectAsync())
                return StatusCode(StatusCodes.Status503ServiceUnavailable,
                    Error.DB_CONNECTION_FAILED);

            if (AuthOptions.IsTokenTerminate(cache, Request))
                return Unauthorized();

            if (await context.Users.FirstOrDefaultAsync(u => u.Username == user.Username)
                is not User updateUser)
                return BadRequest("This user already exist");

            updateUser.Username = user.Username;
            updateUser.Password = HashPassword(user.Password);

            await context.SaveChangesAsync();
            logger.LogInformation($"User info: {GetClaimValue(nameof(user.Username))}; "
                + $"{GetClaimValue(nameof(user.Password))}; change to: {user.Username}; "
                + $"{user.Password}");

            string cacheKey = $"{nameof(User)}{updateUser.Id}";

            cache.Set(cacheKey, updateUser);
            logger.LogInformation($"{cacheKey} add to cache");

            return Ok();
        }

        [HttpGet(nameof(Me))]
        public async Task<ActionResult<User>> Me()
        {
            if (!await context.Database.CanConnectAsync())
                return StatusCode(StatusCodes.Status503ServiceUnavailable,
                    Error.DB_CONNECTION_FAILED);

            if (AuthOptions.IsTokenTerminate(cache, Request))
                return Unauthorized();

            int id = int.Parse(GetClaimValue(nameof(Models.User.Id)));
            string cacheKey = $"{nameof(User)}{id}";

            if (cache.Get(cacheKey) is not User user)
            {
                user = await context.Users.FirstAsync(u => u.Id == id);
                cache.Set(cacheKey, user);
                logger.LogInformation($"{cacheKey} add to cache");
            }
            return Ok(user);
        }

        private static string GenerateToken(User user)
        {
            var claims = new List<Claim>
            {
                new(nameof(user.Id), $"{user.Id}"),
                new(nameof(user.Username), $"{user.Username}"),
                new(nameof(user.Password), $"{user.Password}")
            };
            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(),
                    SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        private static string HashPassword(string password)
            => Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(password)));

        private string GetClaimValue(string type)
            => User.Claims.First(x => x.Type == type).Value;
    }
}
