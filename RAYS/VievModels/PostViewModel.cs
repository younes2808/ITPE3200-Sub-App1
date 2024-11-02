using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RAYS.Models;

namespace RAYS.ViewModels
{
    public class PostViewModel
    {
        public int Id { get; set; } // For editing existing posts

        [Required(ErrorMessage = "Content is required.")]
        public required string Content { get; set; } // Content of the post

        public IFormFile? Image { get; set; } // For image uploads

        [Url(ErrorMessage = "Invalid URL format.")]
        public string? VideoUrl { get; set; } // URL for the video

        [StringLength(100, ErrorMessage = "Location cannot exceed 100 characters.")]
        public string? Location { get; set; } // Location related to the post

        public int UserId { get; set; } // ID of the user creating the post
    }
}
