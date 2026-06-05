using GuayaquilBank.Application.Contracts;
using GuayaquilBank.Application.Dtos.Authentication.Request;
using GuayaquilBank.Application.Dtos.Authentication.Response;
using GuayaquilBank.Domain.Exceptions;
using GuayaquilBank.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GuayaquilBank.Application.Services
{
    public class AuthenticationAppService : IAuthenticationAppService
    {
        private readonly ILogger<AuthenticationAppService> _logger;
        private readonly IApplicationDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtProvider _jwtProvider;

        public AuthenticationAppService(
            ILogger<AuthenticationAppService> logger,
            IApplicationDbContext context,
            IPasswordHasher passwordHasher,
            IJwtProvider jwtProvider)
        {
            _logger = logger;
            _context = context;
            _passwordHasher = passwordHasher;
            _jwtProvider = jwtProvider;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
            _logger.LogInformation("Intentando iniciar sesión para el usuario '{Username}' en el dominio '{Domain}'", request.Username, request.Domain);

            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.Domain.ToLower() == request.Domain.ToLower());

            if (company == null)
            {
                _logger.LogWarning("Intento de login fallido: El dominio '{Domain}' no existe.", request.Domain);
                throw new UserFriendlyException("El dominio, usuario o contraseña son incorrectos.");
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.CompanyId == company.Id && u.Username.ToLower() == request.Username.ToLower());

            if (user == null)
            {
                _logger.LogWarning("Intento de login fallido: El usuario '{Username}' no existe en el dominio '{Domain}'.", request.Username, request.Domain);
                throw new UserFriendlyException("El dominio, usuario o contraseña son incorrectos.");
            }

            bool isPasswordValid = _passwordHasher.VerifyPassword(request.Password, user.PasswordHash);

            if (!isPasswordValid)
            {
                _logger.LogWarning("Intento de login fallido: Contraseña incorrecta para el usuario '{Username}' en '{Domain}'.", request.Username, request.Domain);
                throw new UserFriendlyException("El dominio, usuario o contraseña son incorrectos.");
            }

            var accessToken = _jwtProvider.GenerateToken(user);

            _logger.LogInformation("Sesión iniciada exitosamente para el usuario ID: {UserId}", user.Id);

            return new LoginResponseDto
            {
                Token = accessToken,
                UserId = user.Id,
                Username = user.Username,
                FullName = $"{user.FirstName} {user.LastName}".Trim(),
                CompanyId = company.Id,
                CompanyName = company.Name
            };
        }
    }
}