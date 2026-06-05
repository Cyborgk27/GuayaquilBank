using GuayaquilBank.Application.Contracts;
using GuayaquilBank.Application.Dtos.Profile.Request;
using GuayaquilBank.Application.Dtos.Profile.Response;
using GuayaquilBank.Domain.Entities.Identity;
using GuayaquilBank.Domain.Exceptions;
using GuayaquilBank.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GuayaquilBank.Application.Services
{
    public class ProfileAppService : IProfileAppService
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUser _currentUser;
        private readonly ILogger<ProfileAppService> _logger;

        public ProfileAppService(
            IApplicationDbContext context,
            ICurrentUser currentUser,
            ILogger<ProfileAppService> logger)
        {
            _context = context;
            _currentUser = currentUser;
            _logger = logger;
        }

        public async Task<MyUserResponseDto> GetMyProfileAsync()
        {
            var userId = _currentUser.UserId ?? throw new UserFriendlyException("Su sesión no contiene credenciales de usuario válidas.");

            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new UserFriendlyException("El registro de su perfil de usuario no existe.");

            return MapToUserDto(user);
        }

        public async Task<MyUserResponseDto> UpdateMyProfileAsync(UpdateMyProfileRequestDto request)
        {
            var userId = _currentUser.UserId ?? throw new UserFriendlyException("Sesión caducada o inválida.");

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new UserFriendlyException("No se localizó el perfil a actualizar.");

            var emailTaken = await _context.Users
                .AnyAsync(u => u.Id != userId && u.Email.ToLower() == request.Email.ToLower().Trim());

            if (emailTaken)
                throw new UserFriendlyException("El correo electrónico ingresado ya está en uso por otro operador.");

            user.ChangeEmail(request.Email.Trim());
            user.UpdateProfilePicture(request.ProfilePictureUrl);

            await _context.SaveChangesAsync(default);
            _logger.LogInformation("El usuario {Username} actualizó sus datos de perfil.", user.Username);

            return MapToUserDto(user);
        }

        public async Task<MyCompanyResponseDto> GetMyCompanyAsync()
        {
            var companyId = _currentUser.CompanyId ?? throw new UserFriendlyException("Usted no se encuentra vinculado a ninguna empresa.");

            var company = await _context.Companies
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == companyId);

            if (company == null)
                throw new UserFriendlyException("Los datos comerciales de la empresa no fueron encontrados.");

            return MapToCompanyDto(company);
        }

        public async Task<MyCompanyResponseDto> UpdateMyCompanyAsync(UpdateMyCompanyRequestDto request)
        {
            var companyId = _currentUser.CompanyId ?? throw new UserFriendlyException("Operación denegada: Contexto corporativo ausente.");

            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.Id == companyId);

            if (company == null)
                throw new UserFriendlyException("La empresa asociada a su perfil no existe.");

            var taxIdTaken = await _context.Companies
                .AnyAsync(c => c.Id != companyId && c.TaxId == request.TaxId.Trim());

            if (taxIdTaken)
                throw new UserFriendlyException("La identificación fiscal (TaxId) ya se encuentra registrada por otra entidad jurídica.");

            company.UpdateProfile(request.Name.Trim(), request.Domain.Trim(), request.LogoUrl);
            company.UpdateTaxId(request.TaxId.Trim());
            company.UpdateContactInfo(request.Email.Trim(), request.PhoneNumber.Trim());
            company.UpdateAddress(
                request.Address.Trim(),
                request.City.Trim(),
                request.Region.Trim(),
                request.PostalCode.Trim(),
                request.Country.Trim()
            );
            company.ConfigureFinancialSettings(request.CurrencySymbol.Trim(), request.Iva);

            await _context.SaveChangesAsync(default);
            _logger.LogWarning("Se actualizaron los datos comerciales del Tenant ID: {CompanyId}", companyId);

            return MapToCompanyDto(company);
        }

        private static MyUserResponseDto MapToUserDto(User user)
        {
            return new MyUserResponseDto
            {
                Id = user.Id,
                CompanyId = user.CompanyId,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = user.FullName,
                ProfilePictureUrl = user.ProfilePictureUrl,
                IsActive = user.IsActive
            };
        }

        private static MyCompanyResponseDto MapToCompanyDto(Company company)
        {
            return new MyCompanyResponseDto
            {
                Id = company.Id,
                Name = company.Name,
                TaxId = company.TaxId,
                Domain = company.Domain,
                Email = company.Email,
                PhoneNumber = company.PhoneNumber,
                Address = company.Address,
                City = company.City,
                Region = company.Region,
                PostalCode = company.PostalCode,
                Country = company.Country,
                LogoUrl = company.LogoUrl,
                CurrencySymbol = company.CurrencySymbol,
                Iva = company.Iva,
                IsActive = company.IsActive
            };
        }
    }
}