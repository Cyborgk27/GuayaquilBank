using GuayaquilBank.Domain.Interfaces;
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

            var companies = CompanyFactory.Create(10);
            _context.Companies.AddRange(companies);

            var defaultPasswordHash = _passwordHasher.HashPassword("123qwe");

            foreach (var company in companies)
            {
                var users = UserFactory.Create(company.Id, count: 50, defaultPasswordHash);
                _context.Users.AddRange(users);

                var products = ProductFactory.Create(company.Id, count: 50);
                _context.Products.AddRange(products);

                var allBatchesForCompany = new List<Domain.Entities.Inventory.ProductBatch>();
                var random = new Random();

                foreach (var product in products)
                {
                    var batches = ProductBatchFactory.Create(product.Id, random.Next(1, 100));
                    allBatchesForCompany.AddRange(batches);
                }
                _context.ProductBatches.AddRange(allBatchesForCompany);

                var customers = CustomerFactory.Create(company.Id, count: 5);
                _context.Customers.AddRange(customers);

                foreach (var customer in customers)
                {
                    var invoices = InvoiceFactory.Create(
                        companyId: company.Id,
                        customerId: customer.Id,
                        availableBatches: allBatchesForCompany,
                        count: random.Next(1, 50)
                    );

                    _context.Invoices.AddRange(invoices);
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
