namespace RAYS.ViewModels
{
    public class CommentViewModel
    {
        public int Id { get; set; } // Comment ID
        public string Text { get; set; } = string.Empty; // Comment text
        public DateTime CreatedAt { get; set; } // Date the comment was created
        public int UserId { get; set; } // ID of the user who made the comment
        public int PostId { get; set; } // ID of the post
        public string UserName { get; set; } = string.Empty; // Username of the comment author
        public bool IsEditable { get; set; } // Determines if the logged-in user can edit/delete this comment
    }
}
