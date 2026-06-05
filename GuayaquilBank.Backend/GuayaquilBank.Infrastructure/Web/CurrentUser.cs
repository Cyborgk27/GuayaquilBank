using GuayaquilBank.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace GuayaquilBank.Infrastructure.Web
{
    public class CurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? UserId
        {
            get
            {
                var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (Guid.TryParse(userIdClaim, out var parsedGuid))
                {
                    return parsedGuid;
                }

                return null;
            }
        }

        public Guid? CompanyId
        {
            get
            {
                var companyIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("companyId")?.Value;

                if (Guid.TryParse(companyIdClaim, out var parsedGuid))
                {
                    return parsedGuid;
                }

                return null;
            }
        }

        public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    }
}
