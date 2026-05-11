using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace AfetYonetim.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _http;

        public CurrentUserService(IHttpContextAccessor http)
        {
            _http = http;
        }

        public string? UserId =>
            _http.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        public string? UserName =>
            _http.HttpContext?.User?.Identity?.Name;

        public string? FullName
        {
            get
            {
                var first = _http.HttpContext?.User?.FindFirst("FirstName")?.Value;
                var last = _http.HttpContext?.User?.FindFirst("LastName")?.Value;
                if (!string.IsNullOrWhiteSpace(first) || !string.IsNullOrWhiteSpace(last))
                    return $"{first} {last}".Trim();
                return UserName;
            }
        }

        public bool IsAuthenticated =>
            _http.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    }
}