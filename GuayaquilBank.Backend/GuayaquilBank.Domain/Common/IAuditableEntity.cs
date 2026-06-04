namespace GuayaquilBank.Domain.Common
{
    /// <summary>
    /// Interface para auditar entidades, incluyendo el rastro del usuario.
    /// </summary>
    public interface IAuditable<TUserId> where TUserId : notnull
    {
        DateTime CreatedAtUtc { get; }
        TUserId CreatedBy { get; }

        DateTime? LastModifiedAtUtc { get; }
        TUserId? LastModifiedBy { get; }

        void SetCreation(DateTime createdAtUtc, TUserId createdBy);
        void SetModification(DateTime lastModifiedAtUtc, TUserId lastModifiedBy);
    }
}