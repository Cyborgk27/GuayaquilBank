using Bogus;
using GuayaquilBank.Domain.Entities.Identity;

namespace GuayaquilBank.Infrastructure.Persistence.Factory.Identity
{
    /// <summary>
    /// Factory utilizado para generar datos de prueba realistas para la entidad <see cref="User"/>.
    /// </summary>
    public static class UserFactory
    {
        public static List<User> Create(Guid companyId, int count, string hashedPassword)
        {
            if (companyId == Guid.Empty || count <= 0)
                return new List<User>();

            var faker = new Faker<User>("es")
                .CustomInstantiator(f =>
                {
                    var firstName = f.Name.FirstName();
                    var lastName = f.Name.LastName();

                    var username = $"{firstName[0]}.{lastName}".ToLowerInvariant()
                        .Replace(" ", "")
                        .Replace("ñ", "n");

                    var email = f.Internet.Email(firstName, lastName, "guayaquilbank.com.ec");

                    return new User(
                        id: Guid.NewGuid(),
                        companyId: companyId,
                        username: username,
                        passwordHash: hashedPassword,
                        firstName: firstName,
                        lastName: lastName,
                        email: email
                    );
                });

            return faker.Generate(count);
        }
    }
}