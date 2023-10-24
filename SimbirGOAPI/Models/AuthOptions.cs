using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SimbirGOAPI.Models
{
    public class AuthOptions
    {
        public const string ISSUER = "SimbirGOAPI";
        public const string AUDIENCE = "Client";
        private const string KEY = "SimbirGOAPI";

        public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
            new(SHA256.HashData(Encoding.UTF8.GetBytes(KEY)));

        public static string GetClaimValue(ClaimsPrincipal user, string type)
            => user.Claims.First(x => x.Type == type).Value;
    }
}
