﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimbirGOAPI.Attributes;
using SimbirGOAPI.Extensions;
using SimbirGOAPI.Models;

namespace SimbirGOAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    [ServiceFilter(typeof(DbConnectionAttribute))]
    [ServiceFilter(typeof(CheckBlackListAttribute))]
    public class PaymentController : ControllerBase
    {
        private readonly ILogger<AccountController> logger;
        private readonly SimbirGODbContext context;

        public PaymentController(ILogger<AccountController> logger, SimbirGODbContext context)
        {
            this.logger = logger;
            this.context = context;
        }

        private long UserId => int.Parse(User.GetClaimValue(nameof(Models.User.Role)));

        [HttpPost($"{{{nameof(accountId)}}}")]
        public async Task<IActionResult> Hesoyam(int accountId)
        {
            decimal money = 250000m;
            string nameId = nameof(Models.User.Id);

            if (UserId == (int)RoleEnum.Client && UserId != accountId)
            {
                logger.LogWarning($"User: {nameId}: {UserId}; doesn't have permission");

                return BadRequest("You can add money only yourself");
            }
            context.Users.FirstAsync(u => u.Id == accountId).Result.Balance += money;
            await context.SaveChangesAsync();
            logger.LogInformation($"User: {nameId}: {accountId}; has been added money");

            return Ok();
        }
    }
}
