using GuayaquilBank.Application.Contracts;
using GuayaquilBank.Application.Dtos.Common;
using GuayaquilBank.Application.Dtos.Sales.Request;
using GuayaquilBank.Application.Dtos.Sales.Response;
using GuayaquilBank.Domain.Entities.Sales;
using GuayaquilBank.Domain.Exceptions;
using GuayaquilBank.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GuayaquilBank.Application.Services
{
    public class CustomerAppService : ICustomerAppService
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<CustomerAppService> _logger;
        private readonly ICurrentUser _currentUser;

        public CustomerAppService(
            IApplicationDbContext context,
            ILogger<CustomerAppService> logger,
            ICurrentUser currentUser)
        {
            _context = context;
            _logger = logger;
            _currentUser = currentUser;
        }

        public async Task<CustomerResponseDto> CreateAsync(CreateCustomerRequestDto request)
        {
            var companyId = _currentUser.CompanyId ?? Guid.Empty;
            var identificationClean = request.Identification.Trim();

            _logger.LogInformation("Registrando cliente con Identificación: {Identification} para la empresa: {CompanyId}", identificationClean, companyId);

            var customerExists = await _context.Customers
                .AnyAsync(c => c.CompanyId == companyId && c.Identification == identificationClean);

            if (customerExists)
            {
                throw new UserFriendlyException($"El cliente con RUC/Cédula '{identificationClean}' ya se encuentra registrado.");
            }

            var customer = new Customer(
                id: Guid.NewGuid(),
                companyId: companyId,
                identification: identificationClean,
                fullName: request.FullName.Trim(),
                email: request.Email.Trim(),
                phoneNumber: request.PhoneNumber,
                address: request.Address
            );

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync(default);

            return MapToDto(customer);
        }

        public async Task<PaginationResponseDto<CustomerResponseDto>> GetPagedAsync(PaginationRequestDto request)
        {
            var companyId = _currentUser.CompanyId ?? Guid.Empty;

            var query = _context.Customers
                .Where(c => c.CompanyId == companyId)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
                query = query.Where(c => c.Identification.Contains(search) ||
                                         c.FullName.ToLower().Contains(search) ||
                                         c.Email.ToLower().Contains(search));
            }

            var totalItems = await query.CountAsync();

            var customers = await query
                .OrderBy(c => c.FullName)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var dtos = customers.Select(MapToDto).ToList();

            return new PaginationResponseDto<CustomerResponseDto>(dtos, totalItems, request.Page, request.PageSize);
        }

        public async Task<CustomerResponseDto?> GetByIdAsync(Guid customerId)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Id == customerId && c.CompanyId == _currentUser.CompanyId);

            return customer == null ? null : MapToDto(customer);
        }

        public async Task<CustomerResponseDto> UpdateAsync(Guid customerId, UpdateCustomerRequestDto request)
        {
            var companyId = _currentUser.CompanyId ?? Guid.Empty;
            var identificationClean = request.Identification.Trim();

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Id == customerId && c.CompanyId == companyId);

            if (customer == null)
            {
                throw new UserFriendlyException("El cliente solicitado no existe.");
            }

            var identificationTaken = await _context.Customers
                .AnyAsync(c => c.Id != customerId && c.CompanyId == companyId && c.Identification == identificationClean);

            if (identificationTaken)
            {
                throw new UserFriendlyException($"La identificación '{identificationClean}' ya está asignada a otro cliente.");
            }

            customer.UpdateIdentification(identificationClean);
            customer.UpdateProfile(
                newFullName: request.FullName.Trim(),
                newEmail: request.Email.Trim(),
                newPhoneNumber: request.PhoneNumber,
                newAddress: request.Address
            );

            await _context.SaveChangesAsync(default);
            _logger.LogInformation("Cliente ID: {CustomerId} actualizado de forma exitosa.", customerId);

            return MapToDto(customer);
        }

        public async Task DeleteAsync(Guid customerId)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Id == customerId && c.CompanyId == _currentUser.CompanyId);

            if (customer == null)
            {
                throw new UserFriendlyException("El cliente seleccionado no existe.");
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync(default);
            _logger.LogWarning("Se aplicó borrado lógico al cliente con ID: {CustomerId}", customerId);
        }

        public async Task ToggleStatusAsync(Guid customerId)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Id == customerId && c.CompanyId == _currentUser.CompanyId);

            if (customer == null)
            {
                throw new UserFriendlyException("El cliente seleccionado no existe.");
            }

            if (customer.IsActive)
                customer.Deactivate();
            else
                customer.Activate();

            await _context.SaveChangesAsync(default);
            _logger.LogInformation("Estado del cliente '{FullName}' cambiado a IsActive = {Status}", customer.FullName, customer.IsActive);
        }

        private static CustomerResponseDto MapToDto(Customer customer)
        {
            return new CustomerResponseDto
            {
                Id = customer.Id,
                CompanyId = customer.CompanyId,
                Identification = customer.Identification,
                FullName = customer.FullName,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                Address = customer.Address,
                IsActive = customer.IsActive
            };
        }
    }
}