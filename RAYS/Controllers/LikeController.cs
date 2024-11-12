using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RAYS.Models;
using RAYS.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RAYS.Controllers
{
    [Route("like")]
    [Authorize]
    [ApiController] // Du kan endre dette til ControllerBase hvis det ikke er en API-kontroller
    public class LikeController : Controller
    {
        private readonly LikeService _likeService;

        public LikeController(LikeService likeService)
        {
            _likeService = likeService;
        }

        // POST: like (To Like a Post)
        [HttpPost]
        public async Task<IActionResult> LikePost([FromBody] Like like)
        {
            if (like == null)
            {
                ModelState.AddModelError("", "Invalid like data.");
                return View("Error"); // Returner en feilsiden hvis liken er ugyldig
            }

            var result = await _likeService.LikePostAsync(like);
            if (!result)
            {
                ModelState.AddModelError("", "Post already liked by this user.");
                return View("Error"); // Returner en feilsiden hvis liken allerede eksisterer
            }

            return RedirectToAction("PostDetails", "Post", new { id = like.PostId }); // Forutsatt at du har en PostController med en PostDetails-visning
        }

        // DELETE: like (To Unlike a Post)
        [HttpDelete]
        public async Task<IActionResult> UnlikePost([FromBody] Like like)
        {
            if (like == null)
            {
                ModelState.AddModelError("", "Invalid unlike data.");
                return View("Error"); // Returner en feilsiden hvis liken er ugyldig
            }

            var result = await _likeService.UnlikePostAsync(like);
            if (!result)
            {
                ModelState.AddModelError("", "Like not found.");
                return View("Error"); // Returner en feilsiden hvis liken ikke finnes
            }

            return RedirectToAction("PostDetails", "Post", new { id = like.PostId }); // Forutsatt at du har en PostController med en PostDetails-visning
        }
    }
}
