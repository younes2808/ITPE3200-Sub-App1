using RAYS.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RAYS.ViewModels
{
    public class UserSearchViewModel
    {
        [Display(Name = "Search Query")]
        public required string Query { get; set; } = string.Empty; // Must be provided, defaults to empty

        public List<User> Results { get; set; } = new List<User>(); // Initialize to an empty list
    }
}
