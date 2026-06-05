using System;
using System.Collections.Generic;
using Bogus;
using GuayaquilBank.Domain.Entities.Sales;

namespace GuayaquilBank.Infrastructure.Persistence.Factory.Sales
{
    /// <summary>
    /// Factory utilizado para generar datos de prueba realistas para la entidad <see cref="Customer"/> dentro del contexto de ventas,
    /// </summary>
    public static class CustomerFactory
    {
        public static List<Customer> Create(Guid companyId, int count)
        {
            if (companyId == Guid.Empty || count <= 0)
                return new List<Customer>();

            var faker = new Faker<Customer>("es")
                .CustomInstantiator(f =>
                {
                    var firstName = f.Name.FirstName();
                    var lastName = f.Name.LastName();
                    var fullName = $"{firstName} {lastName}";

                    var identification = f.Random.ReplaceNumbers("09########");

                    var email = f.Internet.Email(firstName, lastName);

                    return new Customer(
                        id: Guid.NewGuid(),
                        companyId: companyId,
                        identification: identification,
                        fullName: fullName,
                        email: email,
                        phoneNumber: f.Phone.PhoneNumber("09########"),
                        address: f.Address.StreetAddress()
                    );
                });

            return faker.Generate(count);
        }
    }
}