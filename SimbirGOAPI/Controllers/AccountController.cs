using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using SimbirGOAPI.Models;
using System.ComponentModel.DataAnnotations;
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
        private readonly List<string> blackList = new();

        private readonly ILogger<AccountController> logger;
        private readonly PostgresContext context;
        private readonly IMemoryCache cache;

        public AccountController(ILogger<AccountController> logger, PostgresContext context,
            IMemoryCache cache)
        {
            this.logger = logger;
            this.context = context;
            this.cache = cache;
        }

        [AllowAnonymous]
        [HttpPost(nameof(SingIn))]
        public async Task<ActionResult<string>> SingIn(UserDTO user)
        {
            if (await context.Users.FirstOrDefaultAsync(u => u.Username == user.Username
                && u.Password == HashPassword(user.Password)) is not User currentUser)
                return BadRequest("User not exist or uncorrected password");

            return Ok(GenerateToken(currentUser));
        }

        [AllowAnonymous]
        [HttpPost(nameof(SingUp))]
        public async Task<IActionResult> SingUp(UserDTO user)
        {
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
            if (IsTokenTerminate())
                return Unauthorized();

            blackList.Add(Request.Headers.Authorization);
            cache.Set(nameof(SecurityToken), blackList, TimeSpan.FromMinutes(2));

            return Ok();
        }

        [HttpPost(nameof(Update))]
        public async Task<IActionResult> Update(UserDTO user)
        {
            if (IsTokenTerminate())
                return Unauthorized();

            if (await context.Users.FirstOrDefaultAsync(u => u.Username == user.Username)
                is not User updateUser)
                return BadRequest("This user already exist");

            updateUser.Username = user.Username;
            updateUser.Password = HashPassword(user.Password);

            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet(nameof(Me))]
        public async Task<ActionResult<User>> Me()
        {
            if (IsTokenTerminate())
                return Unauthorized();

            return Ok(await context.Users.FirstAsync(u => u.Id
                == int.Parse(User.Claims.First().Value)));
        }

        private static string GenerateToken(User user)
        {
            var claims = new List<Claim>
            {
                new(nameof(user.Id), $"{user.Id}")
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

        private bool IsTokenTerminate() =>
            cache.Get(nameof(SecurityToken)) is List<string> blackList
                && blackList.Contains(Request.Headers.Authorization);

        private static string HashPassword(string password) =>
            Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(password)));
    }
}
