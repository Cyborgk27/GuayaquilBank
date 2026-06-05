using GuayaquilBank.Domain.Common;
using GuayaquilBank.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace GuayaquilBank.Infrastructure.Interceptor
{
    /// <summary>
    /// Interceptor que rellena automáticamente los campos de seguimiento de auditoría y gestiona 
    /// las eliminaciones provisionales mediante interfaces de dominio genéricas de tipado fuerte.
    /// </summary>
    public class AuditEntitiesInterceptor : SaveChangesInterceptor
    {
        private readonly ICurrentUser _currentUser;
        private readonly IDateTimeProvider _dateTimeProvider;

        public AuditEntitiesInterceptor(ICurrentUser currentUser, IDateTimeProvider dateTimeProvider)
        {
            _currentUser = currentUser;
            _dateTimeProvider = dateTimeProvider;
        }

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            UpdateAuditProperties(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            UpdateAuditProperties(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void UpdateAuditProperties(DbContext? context)
        {
            if (context == null) return;

            var currentTime = _dateTimeProvider.UtcNow;
            var currentUserId = _currentUser.UserId ?? Guid.Empty;

            foreach (var entry in context.ChangeTracker.Entries())
            {
                if (entry.Entity is IAuditable<Guid> auditableEntity)
                {
                    if (entry.State == EntityState.Added)
                    {
                        var existingCreatedBy = auditableEntity.CreatedBy;
                        var existingCreatedAt = auditableEntity.CreatedAtUtc;

                        var finalUserId = (existingCreatedBy != Guid.Empty && existingCreatedBy != default)
                            ? existingCreatedBy
                            : currentUserId;

                        var finalTime = (existingCreatedAt != default)
                            ? existingCreatedAt
                            : currentTime;

                        auditableEntity.SetCreation(finalTime, finalUserId);
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        auditableEntity.SetModification(currentTime, currentUserId);
                    }
                }

                if (entry.Entity is ISoftDelete<Guid> softDeleteEntity && entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Modified;
                    softDeleteEntity.Delete(currentTime, currentUserId);
                }
            }
        }
    }
}