using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging; 
using RAYS.Models;
using RAYS.Services;
using RAYS.ViewModels;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RAYS.Controllers
{
    [Route("user")]
    public class UserController : Controller
    {
        private readonly UserService _userService;
        private readonly ILogger<UserController> _logger;
        public UserController(UserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        // GET: user/register
        [HttpGet("register")]
        public IActionResult Register()
        {
            return View();
        }

        // POST: user/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegistrationViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return View(request); // Return the view with the model to show validation errors
            }

            try
            {
                await _userService.RegisterUser(request.Username, request.Email, request.Password);
                return RedirectToAction("Login"); // Redirect to login page
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(request); // Return the view with the model to show validation errors
            }
        }

        // GET: user/login
        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }

        // POST: user/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model); // Return the view with the model to show validation errors
            }

            try
            {
                var user = await _userService.LoginUser(model.Username, model.Password);
                if (user != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Username),
                        new Claim("UserId", user.Id.ToString())
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = false,
                        ExpiresUtc = DateTime.UtcNow.AddMinutes(5)
                    };

                    // Sign in the user
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                    // Redirect to the home page after successful login
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            return View(model); // Return the view with the model to show validation errors
        }


        // POST: user/logout
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _logger.LogInformation("User logged out successfully.");
            return RedirectToAction("Login");
        }
    }
}
