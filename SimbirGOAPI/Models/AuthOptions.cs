using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
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

        public static bool IsTokenTerminate(IMemoryCache cache, HttpRequest request)
            => cache.Get(nameof(SecurityToken)) is List<string> blackList
                && blackList.Contains(request.Headers.Authorization);
    }
}
