using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SimbirGOAPI.Extensions
{
    public static class ControllerExtension
    {
        public static T? GetEnumValue<T>(this string value) where T : struct
            => Enum.TryParse(typeof(T), value, out var enumValue) ? (T)enumValue : null;

        public static string GetClaimValue(this ClaimsPrincipal user, string type)
            => user.Claims.First(x => x.Type == type).Value;
    }
}
