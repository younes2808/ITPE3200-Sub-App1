using System;
using System.ComponentModel.DataAnnotations;

namespace RAYS.ViewModels
{
    public class PostViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Content is required")]
    [StringLength(2000, ErrorMessage = "Content cannot exceed 2000 characters.")]
    public string Content { get; set; } = string.Empty;

    public string? ImagePath { get; set; }
    public IFormFile? Image { get; set; } // Image file for upload
    public string? VideoUrl { get; set; }
    public string? Location { get; set; }

    public DateTime CreatedAt { get; set; }

    public int LikeCount { get; set; }
    public int UserId { get; set; }

    public string Username { get; set; } = string.Empty;
    public bool IsLikedByUser { get; set; }
}

}
