using Microsoft.AspNetCore.Mvc;
using RAYS.Models;
using RAYS.Services;
using RAYS.ViewModels;
using System.Threading.Tasks;

namespace RAYS.Controllers
{
    [Route("user")]
    public class UserController : Controller
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        // POST: user/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return validation errors if model state is not valid
            }

            try
            {
                var user = await _userService.RegisterUser(request.Username, request.Email, request.Password);
                return RedirectToAction("UserDetails", new { id = user.Id }); // Redirect to user details view
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState); // Return error messages if registration fails
            }
        }

        // POST: user/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return validation errors if model state is not valid
            }

            try
            {
                var user = await _userService.LoginUser(request.Username, request.Password);
                return RedirectToAction("UserDetails", new { id = user.Id }); // Redirect to user details view
            }
            catch (System.UnauthorizedAccessException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState); // Return error messages if login fails
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetUserById(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            return View("UserDetails", user); // Return the user details view
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchUsers([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Search query cannot be empty.");
            }

            var users = await _userService.SearchUsers(query);
            return View("SearchResults", users.Select(u => new { u.Id, u.Username, u.Email })); // Return the search results view
        }
    }
}
