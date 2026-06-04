namespace GuayaquilBank.Domain.Common
{
    /// <summary>
    /// Interface encargada de definir las propiedades y métodos necesarios para eliminar 
    /// entidades de forma suave en el sistema, registrando el autor de la acción.
    /// </summary>
    public interface ISoftDelete<TUserId> where TUserId : notnull
    {
        DateTime? DeletedAtUtc { get; }
        TUserId? DeletedBy { get; }
        bool IsDeleted { get; }

        void Delete(TUserId deletedBy);
        void UndoDelete();
    }
}
