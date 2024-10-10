using Biograf_Repository.DTO;
using Biograf_Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Repository.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Biograf_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthController(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO userLogin)
        {
            try
            {
                // Validate the input
                if (userLogin == null)
                {
                    return BadRequest("User login cannot be null.");
                }

                if (string.IsNullOrWhiteSpace(userLogin.Email) || string.IsNullOrWhiteSpace(userLogin.Password))
                {
                    return BadRequest("Email and password must be entered.");
                }

                var user = await _userRepository.AuthenticateUserAsync(userLogin.Email, userLogin.Password);
                if (user == null)
                {
                    return Unauthorized("Invalid email or password.");
                }

                var token = GenerateJwtToken(user);
                return Ok(new { token });
            }
            catch (Exception ex)
            {              
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private string GenerateJwtToken(User user)
        {
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Email),         
                new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Jwt:ExpiryMinutes"])),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
