﻿using Microsoft.AspNetCore.Mvc;

namespace SimbirGOAPI
{
    public readonly struct Error
    {
        public static readonly ObjectResult DB_CONNECTION_FAILED = new("Database connection failed")
        {
            StatusCode = StatusCodes.Status503ServiceUnavailable,
        };
    }
}
