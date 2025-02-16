using BackendWebCoreApi.DataTransferObject;
using BackendWebCoreApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BackendWebCoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;
        public AccountController(UserManager<AppUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;

        }
        [HttpPost("NewUser")]
        public async Task<IActionResult> registerNewUser(dtoNewUser user)
        {
            if (ModelState.IsValid)
            {
                AppUser newuser = new()
                {
                    UserName = user.username,
                    Email = user.email,
                    PhoneNumber = user.phoneNumber
                };
                IdentityResult res = await _userManager.CreateAsync(newuser, user.password);
                if (res.Succeeded) return Ok("Succeeded");
                else foreach (var er in res.Errors) ModelState.AddModelError("", er.Description);
            }
            return BadRequest(ModelState);

        }

        [HttpPost("Login")]
        public async Task<IActionResult> login(dtoLogin userlogin)
        {
            if (ModelState.IsValid)
            {
                AppUser? user = await _userManager.FindByNameAsync(userlogin.userName);
                if (user != null)
                {
                    if (await _userManager.CheckPasswordAsync(user, userlogin.password))
                        {
                        //    return Ok("token"); 
                        var claims = new List<Claim>();
                        //claims.Add(new Claim("token", "44"));  // just example i can add any thing
                        claims.Add(new Claim(ClaimTypes.Name, user.UserName));
                        claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
                        claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));  //JWT ID,  generate random unique value, guid: from microsoft, i can use any unique value
                        var rols =await _userManager.GetRolesAsync(user);
                        foreach (var rol in rols) {
                            claims.Add(new Claim(ClaimTypes.Role, rol.ToString()));
                        }
                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes( _configuration["JWT:SecretKey"]));
                        var sc = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                        var token = new JwtSecurityToken(
                            claims: claims,
                            issuer: _configuration["JWT:Issuer"],
                            audience: _configuration["JWT:Audience"],
                            expires: DateTime.Now.AddHours(1),
                            signingCredentials: sc
                            );
                        var _token = new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo
                        };
                        return Ok( _token );
                        }
                    else return Unauthorized();
                }
                else ModelState.AddModelError("", "invalid userName !! ");
            }
            return BadRequest(ModelState);
        }
    }
}
