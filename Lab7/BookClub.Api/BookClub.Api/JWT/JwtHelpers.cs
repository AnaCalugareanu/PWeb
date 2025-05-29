using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BookClub.Api.JWT;
using System.Text.Json;
using System.Text;
using Microsoft.IdentityModel.Tokens;

public static class JwtHelpers
{
    public static string IssueToken(JwtSettings cfg, string role)  
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Role, role)
        };

        var creds = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(cfg.Key)),
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
