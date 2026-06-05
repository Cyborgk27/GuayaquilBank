using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GuayaquilBank.Domain.Interfaces;
using GuayaquilBank.Domain.Entities.Inventory;
using GuayaquilBank.Infrastructure.Persistence.Context;
using GuayaquilBank.Infrastructure.Persistence.Factory.Identity;
using GuayaquilBank.Infrastructure.Persistence.Factory.Inventory;
using GuayaquilBank.Infrastructure.Persistence.Factory.Sales;
using Microsoft.EntityFrameworkCore;

namespace GuayaquilBank.Infrastructure.Persistence.Seeder
{
    /// <summary>
    /// Manejo de la siembra de datos iniciales para la base de datos, incluyendo compañías, 
    /// usuarios, productos, lotes de inventario, clientes y facturas con detalles.
    /// </summary>
    public class ContextSeed
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher _passwordHasher;

        public ContextSeed(ApplicationDbContext context, IPasswordHasher passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public async Task SeedAsync()
        {
            if (await _context.Companies.AnyAsync())
            {
                return;
            }

            var random = new Random();

            var companies = CompanyFactory.Create(10);
            _context.Companies.AddRange(companies);

            var defaultPasswordHash = _passwordHasher.HashPassword("123qwe");

            foreach (var company in companies)
            {
                var users = UserFactory.Create(company.Id, count: 50, defaultPasswordHash);
                _context.Users.AddRange(users);

                var products = ProductFactory.Create(company.Id, count: 50);
                _context.Products.AddRange(products);

                var allBatchesForCompany = new List<ProductBatch>();

                foreach (var product in products)
                {
                    var batchesCount = random.Next(1, 5);
                    var batches = ProductBatchFactory.Create(product.Id, batchesCount);
                    allBatchesForCompany.AddRange(batches);
                }
                _context.ProductBatches.AddRange(allBatchesForCompany);

                var customers = CustomerFactory.Create(company.Id, count: 25);
                _context.Customers.AddRange(customers);

                foreach (var customer in customers)
                {
                    var invoiceCount = random.Next(1, 6);

                    var invoices = InvoiceFactory.Create(
                        companyId: company.Id,
                        customerId: customer.Id,
                        availableUsers: users,
                        availableBatches: allBatchesForCompany,
                        count: invoiceCount
                    );

                    _context.Invoices.AddRange(invoices);
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}