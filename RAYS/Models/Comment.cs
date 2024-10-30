using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace RAYS.Models
{
    public class Comment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonIgnore]  // This hides the Id in JSON responses, including Swagger documentation
        public int Id { get; set; }  // Auto-incremented primary key

        [Required]
        [StringLength(500, ErrorMessage = "Comment text cannot exceed 500 characters.")]
        public required string Text { get; set; }

        [Required]
        public required DateTime CreatedAt { get; set; } = 
            TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Europe/Oslo"));

        [Required]
        [ForeignKey("User")]
        public required int UserId { get; set; }  // Foreign key for User

        [Required]
        [ForeignKey("Post")]
        public required int PostId { get; set; }  // Foreign key for Post
    }
}
