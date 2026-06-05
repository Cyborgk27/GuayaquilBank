namespace GuayaquilBank.Application.Dtos.Sales.Response
{
    public class CustomerResponseDto
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public string Identification { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Address { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}
