using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Api.Helpers
{
    public static class JwtClaimsUtils
    {
        public static int? ObtenerUsuarioId(HttpContext context)
        {
            var claim = context.User.FindFirst(ClaimTypes.NameIdentifier);

            return claim != null ? int.Parse(claim.Value) : null;
        }

        public static string? ObtenerRol(HttpContext context)
        {
            return context.User.FindFirst(ClaimTypes.Role)?.Value;
        }

        public static string? ObtenerCorreo(HttpContext context)
        {
            return context.User.FindFirst(ClaimTypes.Email)?.Value;
        }
    }
}

