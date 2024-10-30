using System;
using System.ComponentModel.DataAnnotations;

namespace RAYS.Models
{
    public class Post
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Content is required")]
        [StringLength(2000, ErrorMessage = "Content cannot exceed 2000 characters.")]
        public string Content { get; set; } = string.Empty; // Ensure it is initialized

        public string? ImagePath { get; set; } // Optional path to the uploaded image

        public string? VideoUrl { get; set; } // Optional URL for a video

        public string? Location { get; set; } // Optional location data

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; } = 
            TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Europe/Oslo"));

        [Required(ErrorMessage = "User ID is required.")]
        public int UserId { get; set; } // Foreign key to the User
    }
}
