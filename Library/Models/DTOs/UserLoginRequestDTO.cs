using Microsoft.Build.Framework;

namespace Library.API.Models.DTOs
{
    public class UserLoginRequestDTO
    {
        [Required]
        public string Email { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
    }
}
