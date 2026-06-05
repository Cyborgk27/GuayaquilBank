using System.ComponentModel.DataAnnotations;

namespace GuayaquilBank.Application.Dtos.Sales.Request
{
    public class UpdateCustomerRequestDto
    {
        [Required] 
        public string Identification { get; set; } = null!;
        [Required] 
        public string FullName { get; set; } = null!;
        [Required][EmailAddress] 
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }
}
