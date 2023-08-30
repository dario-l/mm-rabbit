using Microsoft.IdentityModel.Tokens;

namespace Fast.Shared.Infrastructure.Auth.JWT;

internal sealed record SecurityKeyDetails(SecurityKey Key, string Algorithm);
