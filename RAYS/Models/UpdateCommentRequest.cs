using System.ComponentModel.DataAnnotations;

namespace RAYS.Models
{
    // Define a request model for updating a comment
    public class UpdateCommentRequest
    {
        [Required(ErrorMessage = "Comment ID is required.")]
        public int CommentId { get; set; }
        [Required(ErrorMessage = "Comment text is required.")]
        [StringLength(500, ErrorMessage = "Comment text cannot exceed 500 characters.")]
        public string Text { get; set; } = string.Empty; // Ensure it is initialized
        [Required(ErrorMessage = "User ID is required.")]
        public int UserId { get; set; }

        
    }
}
