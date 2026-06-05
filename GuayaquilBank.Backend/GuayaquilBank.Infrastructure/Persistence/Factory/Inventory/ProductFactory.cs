using Bogus;
using GuayaquilBank.Domain.Entities.Inventory;

namespace GuayaquilBank.Infrastructure.Persistence.Factory.Inventory
{
    /// <summary>
    /// Factory utilizado para generar datos de prueba realistas para la entidad <see cref="Product"/>.
    /// </summary>
    public static class ProductFactory
    {
        public static List<Product> Create(Guid companyId, int count)
        {
            if (companyId == Guid.Empty || count <= 0)
                return new List<Product>();

            var faker = new Faker<Product>("es")
                .CustomInstantiator(f =>
                {
                    var categoryPrefix = f.Commerce.Department().Substring(0, 3).ToUpperBits();
                    var sku = $"{categoryPrefix}-{f.Random.ReplaceNumbers("####")}";

                    return new Product(
                        id: Guid.NewGuid(),
                        companyId: companyId,
                        sku: sku,
                        name: f.Commerce.ProductName(),
                        description: f.Commerce.ProductDescription(),
                        minimumStockAlert: f.Random.Int(5, 20)
                    );
                });

            return faker.Generate(count);
        }
    }

    internal static class StringExtensions
    {
        public static string ToUpperBits(this string value) => value.Trim().ToUpperInvariant();
    }
}