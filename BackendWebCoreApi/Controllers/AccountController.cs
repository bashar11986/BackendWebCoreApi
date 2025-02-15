using BackendWebCoreApi.DataTransferObject;
using BackendWebCoreApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BackendWebCoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        public AccountController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
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
                        return Ok("token");
                    else return Unauthorized();
                }
                else ModelState.AddModelError("", "invalid userName !! ");
            }
            return BadRequest(ModelState);
        }
    }
}
