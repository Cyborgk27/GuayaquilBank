using System.ComponentModel.DataAnnotations;

namespace GuayaquilBank.Application.Dtos.Identity.Request
{
    public class CreateUserRequestDto
    {
        [Required] 
        public string Username { get; set; } = null!;
        [Required] 
        public string Password { get; set; } = null!;
        [Required][EmailAddress]
        public string Email { get; set; } = null!;
        [Required] 
        public string FirstName { get; set; } = null!;
        [Required] 
        public string LastName { get; set; } = null!;
        public string ProfilePictureUrl { get; set; } = string.Empty;
    }
}
