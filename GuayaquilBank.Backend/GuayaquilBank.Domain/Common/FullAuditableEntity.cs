namespace GuayaquilBank.Domain.Common
{
    /// <summary>
    /// Clase base para entidades que requieren auditoría completa y borrado lógico con ID de usuario genérico.
    /// </summary>
    /// <typeparam name="TKey">Tipo de dato para el ID de la entidad (PK).</typeparam>
    /// <typeparam name="TUserId">Tipo de dato para el ID del usuario (Auditoría).</typeparam>
    public class FullAuditableEntity<TKey, TUserId> : AuditableEntity<TKey, TUserId>, ISoftDelete<TUserId>
        where TKey : notnull
        where TUserId : notnull
    {
        public DateTime? DeletedAtUtc { get; private set; }
        public TUserId? DeletedBy { get; private set; }
        public bool IsDeleted { get; private set; }

        protected FullAuditableEntity(TKey id) : base(id) { }
        protected FullAuditableEntity() : base() { }

        public void Delete(DateTime deletedAtUtc, TUserId deletedBy)
        {
            if (IsDeleted) return;

            IsDeleted = true;
            DeletedAtUtc = deletedAtUtc;
            DeletedBy = deletedBy;
        }

        public void UndoDelete()
        {
            if (!IsDeleted) return;

            IsDeleted = false;
            DeletedAtUtc = null;
            DeletedBy = default;
        }
    }
}