using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NewLibrary.Application.Commands.UserCommands;
using NewLibrary.Core.Entities;
using NewLibrary.Data.DAL;
using NewLibrary.Infrastructure.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
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
        private readonly IClaimsManager _claimsManager;
        private readonly AppDbContext _context;

        public AuthController(UserManager<AppUser> userManager, IConfiguration config, RoleManager<IdentityRole> roleManager, IClaimsManager claimsManager, AppDbContext context)
        {
            _userManager = userManager;
            _config = config;
            _roleManager = roleManager;
            _claimsManager = claimsManager;
            _context = context;
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

        [HttpPost]
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

            var claimsIdentity = new ClaimsIdentity();

            claimsIdentity.AddClaim(new Claim("id", user.Id.ToString()));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.GivenName, user.FullName));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
            var roles = await _userManager.GetRolesAsync(user);
            claimsIdentity.AddClaims(roles.Select(r => new Claim(ClaimTypes.Role, r)).ToList());

            var privateKey = Encoding.UTF8.GetBytes(_config["JWTSettings:Key"]);
            var credentials = new SigningCredentials(new SymmetricSecurityKey(privateKey), SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                SigningCredentials = credentials,
                Expires = DateTime.UtcNow.AddHours(1),
                Subject = claimsIdentity,
                Audience = _config.GetSection("JWTSettings:Audience").Value,
                Issuer = _config.GetSection("JWTSettings:Issuer").Value,
            };
            var token = handler.CreateToken(tokenDescriptor);

            return Ok(new { token = handler.WriteToken(token) });
        }
        [HttpGet("current-user")]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                int currentUserId = _claimsManager.GetCurrentUserId();
                var user = await _context.Users.FindAsync(currentUserId);

                if (user == null)
                {
                    return NotFound(new { Message = "User not found" });
                }

                return Ok(user);
            }
            catch (AuthenticationException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An unexpected error occurred", Details = ex.Message });
            }
        }

    }
}
