using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using SimbirGOAPI.Attributes;
using SimbirGOAPI.Extensions;
using SimbirGOAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SimbirGOAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    [ServiceFilter(typeof(DbConnectionAttribute))]
    [ServiceFilter(typeof(CheckBlackListAttribute))]
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> logger;
        private readonly SimbirGODbContext context;
        private readonly List<string> blackList;
        private readonly IMemoryCache cache;

        public AccountController(ILogger<AccountController> logger, SimbirGODbContext context,
            List<string> blackList, IMemoryCache cache)
        {
            this.logger = logger;
            this.context = context;
            this.blackList = blackList;
            this.cache = cache;
        }

        [HttpGet]
        public async Task<ActionResult<User>> Me()
        {
            long id = long.Parse(User.GetClaimValue(nameof(Models.User.Id)));
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
        [AllowAnonymous]
        public async Task<ActionResult<string>> SingIn(UserDTO user)
        {
            if (await context.Users.FirstOrDefaultAsync(u => u.Username == user.Username
                && u.Password == HashPassword(user.Password)) is not User currentUser)
                return BadRequest("User not exist or uncorrected password");

            string token = GenerateToken(currentUser);
            logger.LogInformation($"Generated token: {token}");

            return Ok(token);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SingUp(UserDTO user)
        {
            if (await context.Users.FirstOrDefaultAsync(u => u.Username == user.Username) != null)
                return BadRequest(Error.USER_EXIST);

            await context.Users.AddAsync(new User
            {
                Username = user.Username,
                Password = HashPassword(user.Password),
                Role = (int)RoleEnum.Client,
            });
            await context.SaveChangesAsync();
            logger.LogInformation($"User info: {nameof(user.Username)}: {user.Username}; "
                + $"{nameof(user.Password)}: {user.Password}; has been register");

            return Ok();
        }

        [HttpPost]
        public IActionResult SingOut()
        {
            string token = Request.Headers.Authorization!;

            blackList.Add(token);
            logger.LogInformation($"{nameof(SecurityToken)}: {token} add to {nameof(blackList)}");

            cache.Set(nameof(SecurityToken), blackList, TimeSpan.FromMinutes(2));
            logger.LogInformation($"{nameof(blackList)} add to cache");

            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update(UserDTO user)
        {
            if (await context.Users.FirstOrDefaultAsync(u => u.Username == user.Username) != null)
                return BadRequest(Error.USER_EXIST);

            if (await context.Users.FirstAsync(u => u.Id == long.Parse(User
                .GetClaimValue(nameof(Models.User.Id)))) is not User updateUser)
                return BadRequest(Error.USER_DOESNT_EXIST);

            updateUser.Username = user.Username;
            updateUser.Password = HashPassword(user.Password);

            await context.SaveChangesAsync();
            logger.LogInformation($"User info:\n"
                + $"{nameof(user.Username)}: {User.GetClaimValue(nameof(user.Username))}\n"
                + $"{nameof(user.Password)}: {User.GetClaimValue(nameof(user.Password))}\n"
                + $"change to:\n{nameof(user.Username)}: {user.Username}\n"
                + $"{nameof(user.Password)}: {user.Password}");

            string cacheKey = $"{nameof(User)}{updateUser.Id}";

            cache.Set(cacheKey, updateUser);
            logger.LogInformation($"{cacheKey} add to cache");

            return Ok();
        }

        private string GenerateToken(in User user)
        {
            logger.LogInformation($"Creating token for {nameof(User)}: {user.Username}");

            var claims = new List<Claim>
            {
                new(nameof(user.Id), $"{user.Id}"),
                new(nameof(user.Username), $"{user.Username}"),
                new(nameof(user.Password), $"{user.Password}"),
                new(ClaimsIdentity.DefaultRoleClaimType, $"{user.RoleNavigation.Name}")
            };
            logger.LogInformation($"Claims:\n{nameof(user.Id)}: {user.Id}\n"
                + $"{nameof(user.Username)}: {user.Username}\n"
                + $"{nameof(user.Password)}: {user.Password}\n"
                + $"created");

            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(),
                    SecurityAlgorithms.HmacSha256));

            logger.LogInformation($"Token: {jwt} created");

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        private static string HashPassword(in string password)
            => Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(password)));
    }
}
