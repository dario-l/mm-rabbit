using System.Collections.Generic;
using Fast.Shared.Abstractions.Auth;

namespace Fast.Shared.Infrastructure.Auth.JWT;

public interface IJsonWebTokenManager
{
    JsonWebToken CreateToken(string userId, string? email = null, string? role = null,
        IDictionary<string, IEnumerable<string>>? claims = null);
}