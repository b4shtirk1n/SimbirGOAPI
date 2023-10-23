﻿using Microsoft.AspNetCore.Mvc.Filters;
using SimbirGOAPI.Models;

namespace SimbirGOAPI.Attributes
{
    public class DbConnectionAttribute : IAsyncExceptionFilter
    {
        private readonly ILogger<DbConnectionAttribute> logger;
        private readonly PostgresContext context;

        public DbConnectionAttribute(ILogger<DbConnectionAttribute> logger, PostgresContext context)
        {
            this.context = context;
            this.logger = logger;
        }

        public async Task OnExceptionAsync(ExceptionContext context)
        {
            if (!await this.context.Database.CanConnectAsync())
            {
                logger.LogError($"{Error.DB_CONNECTION_FAILED.Value}");
                context.Result = Error.DB_CONNECTION_FAILED;
            }
        }
    }
}
