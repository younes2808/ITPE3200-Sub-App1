using System.ComponentModel.DataAnnotations;

namespace RAYS.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters.")]
        public required string Username { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Password hash is required.")]
        [StringLength(256, ErrorMessage = "Password hash cannot exceed 256 characters.")]
        public required string PasswordHash { get; set; }
    }
}
