using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BankBranchAPI.Models
{
    public class TokenService
    {
        private readonly IConfiguration _configuration;
        private readonly BankContext _bankContext;

        public TokenService(IConfiguration configuration, BankContext bankContext)
        {
            _bankContext = bankContext;
            _configuration = configuration;
        }

        public (bool IsValid, string Token) GenerateToken(string username, string password)
        {
            var userAccount = _bankContext.UserAccounts.SingleOrDefault(x => x.Username == username);
            if (userAccount == null)
            {
                return (false, "");
            }

            var validPassword = BCrypt.Net.BCrypt.EnhancedVerify(password, userAccount.Password);
            if(!validPassword)
            {
                return (false, "");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
        new Claim(TokenClaimsConstant.Username, username),
        new Claim(TokenClaimsConstant.UserId, userAccount.Id.ToString()),
        new Claim(ClaimTypes.Role, userAccount.IsAdmin ? "Admin" : "User")
        };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials);
            var generatedToken = new JwtSecurityTokenHandler().WriteToken(token);
            return (true, generatedToken);
        }
    }
}
