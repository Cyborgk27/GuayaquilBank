namespace GuayaquilBank.Application.Dtos.Identity.Response
{
    public class UserResponseDto
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string ProfilePictureUrl { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}
