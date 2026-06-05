using System.ComponentModel.DataAnnotations;

namespace GuayaquilBank.Application.Dtos.Identity.Request
{
    public class UpdateUserRequestDto
    {
        [Required][EmailAddress] 
        public string Email { get; set; } = null!;
        [Required] 
        public string FirstName { get; set; } = null!;
        [Required] 
        public string LastName { get; set; } = null!;
        public string ProfilePictureUrl { get; set; } = string.Empty;
    }
}
