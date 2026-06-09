using RGroupConstruction.Application.Interfaces.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace RGroupConstruction.Application.Services;

public class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    public Guid? UserId
    {
        get
        {
            var claim = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(claim, out var id) ? id : null;
        }
    }

    public string IpAddress =>
        httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "::1";
}

