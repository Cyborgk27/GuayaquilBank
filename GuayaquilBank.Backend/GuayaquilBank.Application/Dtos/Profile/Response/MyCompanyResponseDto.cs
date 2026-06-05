namespace GuayaquilBank.Application.Dtos.Profile.Response
{
    public class MyCompanyResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string TaxId { get; set; } = null!;
        public string Domain { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Region { get; set; } = null!;
        public string PostalCode { get; set; } = null!;
        public string Country { get; set; } = null!;
        public string? LogoUrl { get; set; }
        public string CurrencySymbol { get; set; } = "$";
        public decimal Iva { get; set; }
        public bool IsActive { get; set; }
    }
}
