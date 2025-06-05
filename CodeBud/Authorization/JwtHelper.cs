using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CodeBud.DbContext;
using System.Web;
using Microsoft.IdentityModel.Tokens;
using CodeBud.Models.Entities;
using System.Linq;

namespace CodeBud.Helpers
{
    public static class JwtHelper
    {
        private static string SecretKey = "92i;wBGMP:PJ3Jb^n8_Z>d%.Pwl~Hz92<WH^]+S=GzUPCB$Zl{S^sve.HIZ&T!>.2q:!mZT(>*W~TJ{l/=^].`bPX4Pu7r"; // en az 16 karakter

        public static string GenerateToken(int userId, string username)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("userId", userId.ToString()),
                new Claim("username", username)
            };

            var token = new JwtSecurityToken(
                issuer: "codebud.com",
                audience: "codebud.com",
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static ClaimsPrincipal ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(SecretKey);

            var validationParams = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = "codebud.com",
                ValidAudience = "codebud.com",
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuerSigningKey = true
            };

            return tokenHandler.ValidateToken(token, validationParams, out _);
        }
        public static UserModel GetCurrentUserFromToken()
        {
            var token = HttpContext.Current?.Request?.Cookies["jwt_token"]?.Value;
            if (string.IsNullOrEmpty(token))
                return null;

            var principal = ValidateToken(token);
            var userIdClaim = principal?.FindFirst("userId");

            if (userIdClaim == null)
                return null;

            int userId = int.Parse(userIdClaim.Value);
            using (var db = new AppDbContext())
            {
                return db.Users.FirstOrDefault(u => u.Id == userId);
            }
        }
    }
}
