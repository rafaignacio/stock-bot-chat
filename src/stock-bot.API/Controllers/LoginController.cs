using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using stock_bot.API.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using finance_bot.Chatbot.Core;
using System.Threading;

namespace stock_bot.API.Controllers
{
    [Route("oauth")]
    public class LoginController: ControllerBase {
        private readonly IConfiguration _configuration;
        private readonly ILogger<LoginController> _logger;
        private readonly IUserRepository _userRepository;

        public LoginController(IConfiguration configuration, ILogger<LoginController> logger, IUserRepository userRepository) {
            _configuration = configuration;
            _logger = logger;
            _userRepository = userRepository;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserTokenModel>> Login([FromBody] LoginModel loginModel) {
            loginModel.ID = await _userRepository.Login(loginModel.Username, loginModel.Password, CancellationToken.None);

            if(string.IsNullOrEmpty(loginModel.ID)) {
                return Unauthorized();
            }

            var token = GetToken(loginModel);

            return Ok(token);
        }

        [HttpPost("register")]
        public async Task<ActionResult<string>> Register([FromBody] LoginModel loginModel) {
            var id = await _userRepository.Register(loginModel.Username, loginModel.Password, CancellationToken.None);

            //TODO: improve error handling
            if(string.IsNullOrEmpty(id)) {
                return BadRequest();
            } else {
                return Created($"/users/{id}", id);
            }
        }

        public UserTokenModel GetToken(LoginModel loginModel) {
            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, loginModel.ID),
                new Claim(JwtRegisteredClaimNames.UniqueName, loginModel.Username),
            };

            var secretKey = _configuration.GetValue<string>("SecretKey");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials
            );

            var output = new UserTokenModel();

            output.Token = new JwtSecurityTokenHandler().WriteToken(token);

            return output;
        }
    }
}