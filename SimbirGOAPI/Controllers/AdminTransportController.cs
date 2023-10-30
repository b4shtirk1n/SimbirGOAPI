using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using SimbirGOAPI.Attributes;
using SimbirGOAPI.Models;

namespace SimbirGOAPI.Controllers
{
    [ApiController]
    [Route("api/Admin/Transport")]
    [Authorize(Roles = nameof(RoleEnum.Admin))]
    [ServiceFilter(typeof(DbConnectionAttribute))]
    [ServiceFilter(typeof(CheckBlackListAttribute))]
    public class AdminTransportController : ControllerBase
    {
        private readonly ILogger<AdminTransportController> logger;
        private readonly SimbirGODbContext context;
        private readonly IMemoryCache cache;

        public AdminTransportController(ILogger<AdminTransportController> logger,
            SimbirGODbContext context, IMemoryCache cache)
        {
            this.logger = logger;
            this.context = context;
            this.cache = cache;
        }
    }
}
