using Library.API.Configuration;
using Library.API.Models;
using Library.API.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Library.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtConfig _jwtConfig;

        public AuthenticationController(UserManager<IdentityUser> userManager, IOptions<JwtConfig> jwtConfig)
        {
            _userManager = userManager;
            _jwtConfig = jwtConfig.Value;
        }

        [HttpPost]
        [Route(template: "Register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequestDTO requestDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingUser = await _userManager.FindByEmailAsync(requestDTO.Email);
            if (existingUser != null)
            {
                return BadRequest(new AuthResult()
                {
                    Result = false,
                    Errors = new List<string>()
                        {
                            "Email already exist"
                        }
                });
            }

            var newUser = new IdentityUser()
            {
                Email = requestDTO.Email,
                UserName = requestDTO.Email
            };

            var isCreated = await _userManager.CreateAsync(newUser, requestDTO.Password);

            if (isCreated.Succeeded)
            {
                var token = GenerateJwtToken(newUser);
                return Ok(new AuthResult
                {
                    Result = true,
                    Token = token

                });
            }

            return BadRequest(new AuthResult()
            {
                Result = false,
                Errors = isCreated.Errors.Select(e => e.Description).ToList()
            });
        }

        [HttpPost]
        [Route(template: "Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequestDTO requestDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(requestDTO.Email);

            if (user != null)
            {
                var isValidPassword = await _userManager.CheckPasswordAsync(user, requestDTO.Password);
                if (isValidPassword)
                {
                    var token = GenerateJwtToken(user);
                    return Ok(new AuthResult
                    {
                        Result = true,
                        Token = token
                    });
                }
            }

            return BadRequest(new AuthResult
            {
                Result = false,
                Errors = new List<string>()
                    {
                        "Invalid Credentials"
                    }
            });
        }

        private string GenerateJwtToken(IdentityUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var signinKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.SecretKey));

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Aud, _jwtConfig.Audience),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString())
                }),

                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(signinKey, SecurityAlgorithms.HmacSha256)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);

            return jwtToken;
        }
    }

}
