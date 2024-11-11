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
            if (ModelState.IsValid && !string.IsNullOrWhiteSpace(model.Query))
            {
                var results = await _userService.SearchUsersAsync(model.Query);
                model.Results = results;
            }
            return View(model); // Returner samme view med oppdatert modell som inneholder resultater
        }
    }
}
