using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BookClub.Api.JWT;
using System.Text.Json;
using System.Text;
using Microsoft.IdentityModel.Tokens;

public static class JwtHelpers
{
    public static string IssueToken(JwtSettings cfg,
                                    string? role = null,
                                    IEnumerable<string>? perms = null)
    {
        if (role is null && perms is null)
            throw new ArgumentException("Must supply role or permissions");

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // mutually exclusive styles, pick what caller sent
        if (role is not null)
            claims.Add(new Claim(ClaimTypes.Role, role));
        else if (perms is not null)
            claims.Add(new Claim("perms",
                     JsonSerializer.Serialize(perms)));

        var creds = new SigningCredentials(
                        new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(cfg.Key)),
                        SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: cfg.Issuer,
            audience: cfg.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(cfg.ExpiresMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
