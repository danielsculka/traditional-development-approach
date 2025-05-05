using ManualProg.Api.Data.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ManualProg.Api.Features.Auth.Services;

public class IdentityService(IOptions<AuthOptions> options)
{
    private readonly AuthOptions _options = options.Value;

    public string GenerateToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_options.TokenSecurityKey);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Role, user.Role.ToString()),
            new(ClaimTypesExtensions.ProfileId, user.ProfileId?.ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(10),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public bool VerifyPassword(User user, string password)
    {
        if (string.IsNullOrEmpty(password))
            return false;

        var hasher = new PasswordHasher<User>();

        PasswordVerificationResult passwordCheck = hasher.VerifyHashedPassword(user, user.Password, password);

        return passwordCheck == PasswordVerificationResult.Success;
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[2];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}
