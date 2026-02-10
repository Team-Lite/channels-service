using System.Security.Claims;

namespace ChannelsService.Bootstrap.Extensions;

internal static class ClaimsPrincipalExtensions
{
    extension(ClaimsPrincipal principal)
    {
        public Guid GetUserId() => Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}