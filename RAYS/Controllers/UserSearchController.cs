using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RAYS.Models;
using RAYS.Services;
using RAYS.ViewModels;
using System.Threading.Tasks;

namespace RAYS.Controllers
{
    [Authorize]
    public class UserSearchController : Controller
    {
        private readonly UserSearchService _userService;

        public UserSearchController(UserSearchService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Search()
        {
            return View(new UserSearchViewModel { Query = string.Empty });
        }

        [HttpPost]
        public async Task<IActionResult> Search(UserSearchViewModel model)
        {
            if (ModelState.IsValid)
            {

                // Perform the search if the model is valid and Query is not empty
                var results = await _userService.SearchUsersAsync(model.Query);
                model.Results = results;
            }
            else
            {
                // Set an error message if model validation fails
                TempData["ErrorMessage"] = "Search field cannot be empty. Please try again.";
            }

            return View(model); // Return the view (with results or error message)
        }


    }
}
