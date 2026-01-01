using System.Security.Claims;

namespace Web.Server.Services;

public interface IUserContext
{
    Guid UserId { get; }
}

public class HttpUserContext : IUserContext
{
    private readonly IHttpContextAccessor _context;

    public HttpUserContext(IHttpContextAccessor httpContextAccessor)
    {
        _context = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    public Guid UserId
    {
        get
        {
            var userId = string.Empty;
            if (string.IsNullOrEmpty(userId))
            {
                userId = _context.HttpContext?.User.FindFirst("sub")?.Value;
            }
            if (string.IsNullOrEmpty(userId))
            {
                userId = _context.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }
            return string.IsNullOrEmpty(userId) ? throw new InvalidOperationException("User ID is not available") : Guid.Parse(userId);
        }
    }
}
