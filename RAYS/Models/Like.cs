using System.ComponentModel.DataAnnotations;

namespace RAYS.Models
{
    public class Like
    {
        [Required]
        public int UserId { get; set; }  // Foreign key to User

        [Required]
        public int PostId { get; set; }   // Foreign key to Post

        [Required]
        public DateTime LikedAt { get; set; } = 
            TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Europe/Oslo"));
    }
}
