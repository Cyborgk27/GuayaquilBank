using GuayaquilBank.Application.Contracts;
using GuayaquilBank.Application.Dtos.Common;
using GuayaquilBank.Application.Dtos.Identity.Request;
using GuayaquilBank.Application.Dtos.Identity.Response;
using GuayaquilBank.Domain.Entities.Identity;
using GuayaquilBank.Domain.Exceptions;
using GuayaquilBank.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GuayaquilBank.Application.Services
{
    public class UserAppService : IUserAppService
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<UserAppService> _logger;
        private readonly ICurrentUser _currentUser;
        private readonly IPasswordHasher _passwordHasher;

        public UserAppService(
            IApplicationDbContext context,
            ILogger<UserAppService> logger,
            ICurrentUser currentUser,
            IPasswordHasher passwordHasher)
        {
            _context = context;
            _logger = logger;
            _currentUser = currentUser;
            _passwordHasher = passwordHasher;
        }

        public async Task<UserResponseDto> CreateAsync(CreateUserRequestDto request)
        {
            var companyId = _currentUser.CompanyId ?? Guid.Empty;
            _logger.LogInformation("Registrando nuevo usuario '{Username}' para el Tenant ID: {CompanyId}", request.Username, companyId);

            var userExists = await _context.Users
                .AnyAsync(u => u.Username.ToLower() == request.Username.ToLower().Trim() || u.Email.ToLower() == request.Email.ToLower().Trim());

            if (userExists)
            {
                throw new UserFriendlyException("El nombre de usuario o correo electrónico ya se encuentra registrado en el sistema.");
            }

            string passwordHash = _passwordHasher.HashPassword(request.Password);

            var user = new User(
                id: Guid.NewGuid(),
                companyId: companyId,
                username: request.Username.Trim(),
                passwordHash: passwordHash,
                email: request.Email.Trim(),
                firstName: request.FirstName.Trim(),
                lastName: request.LastName.Trim(),
                profilePictureUrl: request.ProfilePictureUrl
            );

            _context.Users.Add(user);
            await _context.SaveChangesAsync(default);

            return MapToDto(user);
        }

        public async Task<PaginationResponseDto<UserResponseDto>> GetPagedAsync(PaginationRequestDto request)
        {
            var companyId = _currentUser.CompanyId ?? Guid.Empty;

            var query = _context.Users
                .Where(u => u.CompanyId == companyId)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
                query = query.Where(u => u.Username.ToLower().Contains(search) ||
                                         u.FirstName.ToLower().Contains(search) ||
                                         u.LastName.ToLower().Contains(search) ||
                                         u.Email.ToLower().Contains(search));
            }

            var totalItems = await query.CountAsync();

            var users = await query
                .OrderBy(u => u.LastName)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var dtos = users.Select(MapToDto).ToList();

            return new PaginationResponseDto<UserResponseDto>(dtos, totalItems, request.Page, request.PageSize);
        }

        public async Task<UserResponseDto?> GetByIdAsync(Guid userId)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId && u.CompanyId == _currentUser.CompanyId);

            return user == null ? null : MapToDto(user);
        }

        public async Task<UserResponseDto> UpdateAsync(Guid userId, UpdateUserRequestDto request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId && u.CompanyId == _currentUser.CompanyId);

            if (user == null)
            {
                throw new UserFriendlyException("El usuario solicitado para actualizar no existe.");
            }

            var emailTaken = await _context.Users
                .AnyAsync(u => u.Id != userId && u.Email.ToLower() == request.Email.ToLower().Trim());

            if (emailTaken)
            {
                throw new UserFriendlyException("El correo electrónico ingresado ya pertenece a otro usuario.");
            }

            user.ChangeEmail(request.Email.Trim());
            user.UpdateProfilePicture(request.ProfilePictureUrl);

            await _context.SaveChangesAsync(default);
            _logger.LogInformation("Usuario ID: {UserId} actualizado con éxito.", userId);

            return MapToDto(user);
        }

        public async Task DeleteAsync(Guid userId)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId && u.CompanyId == _currentUser.CompanyId);

            if (user == null)
            {
                throw new UserFriendlyException("El usuario seleccionado no existe.");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync(default);
            _logger.LogWarning("Se aplicó borrado lógico al usuario con ID: {UserId}", userId);
        }

        public async Task ToggleStatusAsync(Guid userId)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId && u.CompanyId == _currentUser.CompanyId);

            if (user == null)
            {
                throw new UserFriendlyException("El usuario seleccionado no existe.");
            }

            if (user.IsActive)
                user.Deactivate();
            else
                user.Activate();

            await _context.SaveChangesAsync(default);
            _logger.LogInformation("Se cambió el estado de actividad del usuario {Username} a: {Status}", user.Username, user.IsActive);
        }

        private static UserResponseDto MapToDto(User user)
        {
            return new UserResponseDto
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
    }
}