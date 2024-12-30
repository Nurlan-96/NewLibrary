using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NewLibrary.Application.Commands.UserCommands;
using NewLibrary.Core.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NewLibrary.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _config;

        public AuthController(UserManager<AppUser> userManager, IConfiguration config, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _config = config;
            _roleManager = roleManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterCommand urc)
        {
            var existUser = await _userManager.FindByNameAsync(urc.UserName);
            if (existUser != null) return Conflict();
            AppUser user = new()
            {
                Email = urc.Email,
                UserName = urc.UserName,
                FullName = urc.FullName,
            };
            var result = await _userManager.CreateAsync(user, urc.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);
            await _userManager.AddToRoleAsync(user, "member");
            return StatusCode(201);
        }

        [HttpGet]
        public async Task<IActionResult> CreateRole()
        {
            if (!await _roleManager.RoleExistsAsync("member"))
            {
                await _roleManager.CreateAsync(new IdentityRole() { Name = "member" });
            }
            if (!await _roleManager.RoleExistsAsync("admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole() { Name = "admin" });
            }
            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginCommand ulc)
        {
            var user = await _userManager.FindByNameAsync(ulc.Username);
            if (user == null) return BadRequest();
            var result = await _userManager.CheckPasswordAsync(user, ulc.Password);
            if (!result) return BadRequest();

            var handler = new JwtSecurityTokenHandler();

            var ci = new ClaimsIdentity();

            ci.AddClaim(new Claim("id", user.Id.ToString()));
            ci.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
            ci.AddClaim(new Claim(ClaimTypes.GivenName, user.FullName));
            ci.AddClaim(new Claim(ClaimTypes.Email, user.Email));
            var roles = await _userManager.GetRolesAsync(user);
            ci.AddClaims(roles.Select(r => new Claim(ClaimTypes.Role, r)).ToList());

            var privateKey = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
            var credentials = new SigningCredentials(new SymmetricSecurityKey(privateKey), SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                SigningCredentials = credentials,
                Expires = DateTime.UtcNow.AddHours(1),
                Subject = ci,
                Audience = _config.GetSection("Jwt:Audience").Value,
                Issuer = _config.GetSection("Jwt:Issuer").Value,
            };
            var token = handler.CreateToken(tokenDescriptor);


            return Ok(new { token = handler.WriteToken(token) });
        }
    }
}
