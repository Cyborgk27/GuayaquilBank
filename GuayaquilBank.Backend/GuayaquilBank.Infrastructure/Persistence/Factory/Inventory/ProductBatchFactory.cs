using Bogus;
using GuayaquilBank.Domain.Entities.Inventory;

namespace GuayaquilBank.Infrastructure.Persistence.Factory.Inventory
{
    /// <summary>
    /// Factory utilizado para generar datos de prueba realistas para la entidad <see cref="ProductBatch"/>.
    /// </summary>
    public static class ProductBatchFactory
    {
        public static List<ProductBatch> Create(Guid productId, int count)
        {
            if (productId == Guid.Empty || count <= 0)
                return new List<ProductBatch>();

            var faker = new Faker<ProductBatch>("es")
                .CustomInstantiator(f =>
                {
                    var batchNumber = $"LOT-{f.Date.Recent().Year}-{f.Random.ReplaceNumbers("###")}";

                    var manufacturedAt = f.Date.Past(1);
                    var expirationDate = f.Date.Future(2, manufacturedAt);

                    return new ProductBatch(
                        id: Guid.NewGuid(),
                        productId: productId,
                        batchNumber: batchNumber,
                        quantity: f.Random.Int(10, 150),
                        unitCost: decimal.Parse(f.Commerce.Price(1.50m, 200.00m, 2)),
                        manufacturedAt: manufacturedAt,
                        expirationDate: expirationDate
                    );
                });

            return faker.Generate(count);
        }
    }
}