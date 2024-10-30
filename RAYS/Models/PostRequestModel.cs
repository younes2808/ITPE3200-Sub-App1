using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace RAYS.Models
{
    public class PostRequestModel
    {
        [Required(ErrorMessage = "Content is required.")]
        public string Content { get; set; } = string.Empty; // Ensure it is initialized

        public IFormFile? Image { get; set; } // For image uploads (optional)

        public string? Location { get; set; } // Optional location field

        [Required(ErrorMessage = "User ID is required.")]
        public int UserId { get; set; } // User ID for the post author

        public string? VideoUrl { get; set; } // Optional property for the video link
    }
}
