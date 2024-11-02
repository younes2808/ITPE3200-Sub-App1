namespace RAYS.ViewModels
{

    public class CommentsPageViewModel
    {
        public int PostId { get; set; } // The ID of the post for which comments are being displayed
        public IEnumerable<CommentWithUserViewModel> Comments { get; set; } // List of comments for the post

        public CommentsPageViewModel()
        {
            Comments = new List<CommentWithUserViewModel>();
        }
    }
    public class CommentWithUserViewModel
    {
        public int Id { get; set; }
        public required string Text { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UserId { get; set; }
        public int PostId { get; set; }
        public required string UserName { get; set; } // Add this property for the username
    }
}
