using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BookClub.Api.JWT
{
    public record JwtSettings(string Issuer,
                          string Audience,
                          string Key,
                          int ExpiresMinutes)
    {
        public TokenValidationParameters AsValidationParams() => new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = Issuer,
            ValidAudience = Audience,
            IssuerSigningKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key)),
            ClockSkew = TimeSpan.Zero           // no extra grace
        };
    }
}
