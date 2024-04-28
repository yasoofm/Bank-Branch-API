using BankBranchAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankBranchAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LoginController : Controller
    {
        private readonly TokenService _tokenService;
        private readonly BankContext _bankContext;
        public LoginController(TokenService tokenService, BankContext bankContext) 
        { 
            _bankContext = bankContext;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLoginRequest user)
        {
            var response = _tokenService.GenerateToken(user.Username, user.Password);
            if(response.IsValid)
            {
                return Ok(new { Token = response.Token });
            }
            return BadRequest("Username and/or Password is incorrect");
        }

        [HttpPost("Register")]
        public IActionResult Register([FromBody] UserRegistration userRegistration)
        {
            bool isAdmin = false;
            if (_bankContext.UserAccounts.Count() == 0)
            {
                isAdmin = true;
            }

            var newAccount = UserAccount.Create(userRegistration.UserName, userRegistration.Password, isAdmin);

            _bankContext.UserAccounts.Add(newAccount);
            _bankContext.SaveChanges();

            return Ok(new { Message = "User Created" });
        }
    }
}
