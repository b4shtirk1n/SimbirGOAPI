using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;

namespace SimbirGOAPI.Attributes
{
    public class CheckBlackListAttribute : IAuthorizationFilter
    {
        private readonly ILogger<CheckBlackListAttribute> logger;
        private readonly IMemoryCache cache;

        public CheckBlackListAttribute(ILogger<CheckBlackListAttribute> logger, IMemoryCache cache)
        {
            this.logger = logger;
            this.cache = cache;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (cache.Get(nameof(SecurityToken)) is List<string> blackList
                && blackList.Contains(context.HttpContext.Request.Headers.Authorization))
            {
                logger.LogError($"{Error.TOKEN_TERMINATED.Value}");
                context.Result = Error.TOKEN_TERMINATED;
            }
        }
    }
}
