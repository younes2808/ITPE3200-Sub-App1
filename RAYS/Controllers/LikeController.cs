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
    [ApiController]
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
                return View("Error"); // Returns error if like is null
            }

            var result = await _likeService.LikePostAsync(like);
            if (!result)
            {
                ModelState.AddModelError("", "Post already liked by this user.");
                return View("Error"); // Returns an error if Like already exists
            }

            return RedirectToAction("PostDetails", "Post", new { id = like.PostId });
        }

        // DELETE: like (To Unlike a Post)
        [HttpDelete]
        public async Task<IActionResult> UnlikePost([FromBody] Like like)
        {
            if (like == null)
            {
                ModelState.AddModelError("", "Invalid unlike data.");
                return View("Error"); // Returns error if like is invalid
            }

            var result = await _likeService.UnlikePostAsync(like);
            if (!result)
            {
                ModelState.AddModelError("", "Like not found.");
                return View("Error"); // Returns error if like doesnt exist
            }

            return RedirectToAction("PostDetails", "Post", new { id = like.PostId });
        }
    }
}
