using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CSWWeb.Lib.Utils
{
    public class JwtHelper
    {
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JwtHelper(IConfiguration configuration,IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _secretKey = configuration["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key is missing");
            _issuer = configuration["Jwt:Issuer"] ?? "DefaultIssuer";
            _audience = configuration["Jwt:Audience"] ?? "DefaultAudience";
        }
        /// <summary>
        /// 產生 JWT Token
        /// </summary>
        public string CreateToken(string username, List<Claim> additionalClaims = null, int expireMinutes = 60)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim> { new Claim(ClaimTypes.Name, username) };
            if (additionalClaims != null) claims.AddRange(additionalClaims);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expireMinutes),
                signingCredentials: credentials
            );
            
            var tokenHandler = new JwtSecurityTokenHandler();
            var token_result = tokenHandler.WriteToken(token);
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = System.TimeSpan.Zero
            };
            var principal = tokenHandler.ValidateToken(token_result, validationParameters, out SecurityToken validatedToken);
            _httpContextAccessor.HttpContext.User = principal;

            return token_result;
        }
    }
}
