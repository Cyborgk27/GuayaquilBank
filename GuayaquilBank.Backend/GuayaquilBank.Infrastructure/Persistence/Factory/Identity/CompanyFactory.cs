using Bogus;
using GuayaquilBank.Domain.Entities.Identity;

namespace GuayaquilBank.Infrastructure.Persistence.Factory.Identity
{
    /// <summary>
    /// Factory utilizado para generar datos de prueba realistas para la entidad <see cref="Company"/>.
    /// </summary>
    public static class CompanyFactory
    {
        public static List<Company> Create(int count)
        {
            if (count <= 0) return new List<Company>();

            var faker = new Faker<Company>("es")
                .CustomInstantiator(f =>
                {
                    var companyName = f.Company.CompanyName();
                    var cleanName = companyName.Replace(" ", "").Replace(",", "").ToLower();
                    var domain = $"{cleanName}.com.ec";

                    var taxId = f.Random.ReplaceNumbers("09#########01");

                    return new Company(
                        id: Guid.NewGuid(),
                        name: companyName,
                        taxId: taxId,
                        domain: domain,
                        email: f.Internet.Email(cleanName),
                        phoneNumber: f.Phone.PhoneNumber("04######"),
                        address: f.Address.StreetAddress(),
                        city: f.Address.City(),
                        region: f.Address.State(),
                        postalCode: f.Address.ZipCode("######"),
                        country: "Ecuador",
                        logoUrl: f.Internet.Avatar(),
                        currencySymbol: "$",
                        iva: 0.15m
                    );
                });

            return faker.Generate(count);
        }
    }
}