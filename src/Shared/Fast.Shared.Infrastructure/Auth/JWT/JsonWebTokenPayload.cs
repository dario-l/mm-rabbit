using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Fast.Shared.Infrastructure.Auth.JWT;

internal sealed class JsonWebTokenPayload
{
    public string Subject { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? Email { get; set; }
    public DateTime Expiry { get; set; }
    public IEnumerable<string> Roles { get; set; } = Enumerable.Empty<string>();
    public IEnumerable<Claim> Claims { get; set; } = Enumerable.Empty<Claim>();
}