using System.ComponentModel.DataAnnotations;

public class ChangePasswordRequest
{
    [Required]
    [StringLength(50, MinimumLength = 5)]
    public required string Username { get; set; }

    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Old password must be at least 8 characters long.")]
    public required string OldPassword { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "New password must be at least 8 characters long.")]
    public required string NewPassword { get; set; }
}
