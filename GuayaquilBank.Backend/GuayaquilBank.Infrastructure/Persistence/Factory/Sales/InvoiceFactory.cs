using System;
using System.Collections.Generic;
using Bogus;
using GuayaquilBank.Domain.Entities.Identity;
using GuayaquilBank.Domain.Entities.Inventory;
using GuayaquilBank.Domain.Entities.Sales;

namespace GuayaquilBank.Infrastructure.Persistence.Factory.Sales
{
    /// <summary>
    /// Factory utilizado para generar datos de prueba realistas para la entidad <see cref="Invoice"/> y sus detalles asociados.
    /// </summary>
    public static class InvoiceFactory
    {
        public static List<Invoice> Create(Guid companyId, Guid customerId, List<User> availableUsers, List<ProductBatch> availableBatches, int count)
        {
            if (companyId == Guid.Empty ||
                customerId == Guid.Empty ||
                availableUsers == null || availableUsers.Count == 0 ||
                availableBatches == null || availableBatches.Count == 0 ||
                count <= 0)
            {
                return new List<Invoice>();
            }

            var faker = new Faker<Invoice>("es")
                .CustomInstantiator(f =>
                {
                    var invoiceNumber = $"{f.Random.ReplaceNumbers("00#")}-{f.Random.ReplaceNumbers("00#")}-{f.Random.ReplaceNumbers("#########")}";

                    var issuedAt = f.Date.Recent(30);

                    var randomUser = f.PickRandom(availableUsers);

                    var invoice = new Invoice(
                        id: Guid.NewGuid(),
                        companyId: companyId,
                        customerId: customerId,
                        invoiceNumber: invoiceNumber,
                        issuedAtUtc: issuedAt
                    );

                    invoice.SetCreation(issuedAt, randomUser.Id);

                    var detailsCount = f.Random.Int(1, Math.Min(10, availableBatches.Count));
                    var shuffledBatches = f.PickRandom(availableBatches, detailsCount);

                    foreach (var batch in shuffledBatches)
                    {
                        var randomMarginMultiplier = (decimal)f.Random.Double(1.15, 1.40);
                        var unitPrice = Math.Round(batch.UnitCost * randomMarginMultiplier, 2);

                        invoice.AddItem(
                            detailId: Guid.NewGuid(),
                            productBatchId: batch.Id,
                            quantity: f.Random.Int(1, 20),
                            unitPrice: unitPrice,
                            taxPercentage: 0.15m
                        );
                    }

                    return invoice;
                });

            return faker.Generate(count);
        }
    }
}