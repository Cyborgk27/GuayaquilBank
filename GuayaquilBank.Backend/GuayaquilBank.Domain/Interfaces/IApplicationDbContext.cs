using GuayaquilBank.Domain.Entities.Identity;
using GuayaquilBank.Domain.Entities.Inventory;
using GuayaquilBank.Domain.Entities.Sales;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuayaquilBank.Domain.Interfaces
{
    /// <summary>
    /// Abstracción para el contexto de la base de datos de la aplicación, definiendo los conjuntos de 
    /// entidades y el método para guardar cambios. Esta interfaz es fundamental para la implementación 
    /// del patrón de repositorio y la inyección de dependencias, permitiendo una separación clara entre 
    /// la lógica de acceso a datos y el resto de la aplicación. Al definir esta interfaz, se facilita la 
    /// creación de implementaciones concretas que interactúan con diferentes proveedores de bases de datos 
    /// o que pueden ser fácilmente simuladas para pruebas unitarias.
    /// </summary>
    public interface IApplicationDbContext
    {
        DbSet<Company> Companies { get; }
        DbSet<User> Users { get; }
        DbSet<Product> Products { get; }
        DbSet<ProductBatch> ProductBatches { get; }
        DbSet<Customer> Customers { get; }
        DbSet<Invoice> Invoices { get; }
        DbSet<InvoiceDetail> InvoiceDetails { get; }

        /// <summary>
        /// Guarda los cambios realizados en el contexto de la base de datos de forma asíncrona, 
        /// con soporte para cancelación a través de un token. Este método es esencial para persistir 
        /// las modificaciones
        /// </summary>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
