namespace GuayaquilBank.Domain.Common
{
    /// <summary>
    /// Clase base para entidades que requieren auditoría con ID de usuario genérico.
    /// </summary>
    /// <typeparam name="TId">Tipo de dato para el ID de la entidad.</typeparam>
    /// <typeparam name="TUserId">Tipo de dato para el ID del usuario auditor.</typeparam>
    public abstract class AuditableEntity<TId, TUserId> : Entity<TId>, IAuditable<TUserId>
        where TId : notnull
        where TUserId : notnull
    {
        public DateTime CreatedAtUtc { get; private set; }
        public TUserId CreatedBy { get; private set; } = default!;

        public DateTime? LastModifiedAtUtc { get; private set; }
        public TUserId? LastModifiedBy { get; private set; }

        protected AuditableEntity(TId id) : base(id) { }
        protected AuditableEntity() : base() { }

        public void SetCreation(DateTime createdAtUtc, TUserId createdBy)
        {
            CreatedAtUtc = createdAtUtc;
            CreatedBy = createdBy;
        }

        public void SetModification(DateTime lastModifiedAtUtc, TUserId lastModifiedBy)
        {
            LastModifiedAtUtc = lastModifiedAtUtc;
            LastModifiedBy = lastModifiedBy;
        }
    }
}