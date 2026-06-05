namespace GuayaquilBank.Domain.Common
{
    /// <summary>
    /// Clase base abstracta que representa una entidad en el sistema, proporcionando una implementación común para la comparación de entidades basada en su identificador único.
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    public abstract class Entity<TId> : IEquatable<Entity<TId>>
    where TId : notnull
    {
        public TId Id { get; protected set; }

        protected Entity(TId id)
        {
            Id = id;
        }

#pragma warning disable CS8618
        protected Entity() { }
#pragma warning restore CS8618

        public override bool Equals(object? obj)
        {
            return obj is Entity<TId> entity && Equals(entity);
        }

        public bool Equals(Entity<TId>? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id.Equals(other.Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode() * 41;
        }

        public static bool operator ==(Entity<TId>? left, Entity<TId>? right)
        {
            if (left is null && right is null) return true;
            if (left is null || right is null) return false;
            return left.Equals(right);
        }

        public static bool operator !=(Entity<TId>? left, Entity<TId>? right)
        {
            return !(left == right);
        }
    }
}
