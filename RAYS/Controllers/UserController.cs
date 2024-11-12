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

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegistrationViewModel request)
        {
            if (!ModelState.IsValid)
            {
                // Validation failed, return the view with errors.
                return View(request);
            }

            try
            {
                await _userService.RegisterUser(request.Username, request.Email, request.Password);
                return RedirectToAction("Login"); // Redirect to login page after successful registration
            }
            catch (System.Exception ex)
            {
                // Add exception message to ModelState, which will be displayed in the view
                ModelState.AddModelError("", ex.Message);
                return View(request); // Return the view with the model and error messages
            }
        }


        // GET: user/login
        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // If the model is invalid, return the view with model errors
                return View(model);
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
                    return RedirectToAction("Index", "Post");
                }
            }
            catch (System.Exception ex)
            {
                // Add the exception message to ModelState so it can be shown in the view
                ModelState.AddModelError("", ex.Message);
            }

            // If there was an error or unsuccessful login, return the model with errors back to the view
            return View(model);
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
