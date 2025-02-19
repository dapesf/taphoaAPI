using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

public static class JwtTokenCreateModule
{
    public static string secretKey = ConfigurationManager.AppSetting["SecretKey"] ?? "";
    
    public static void JWTConfig()
    {
        TokenValidationParameters option = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "http://localhost:3000",
            ValidAudience = "http://localhost:3000",
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secretKey))
        };
    }

    public static string GenerateToken(string id)
    {
        var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
                new Claim(ClaimTypes.NameIdentifier, id),
                new Claim(ClaimTypes.Role, id)
            };
        var token = new JwtSecurityToken(
            "http://localhost:3000",
            "http://localhost:3000",
            claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: credentials);


        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}