namespace Domain.Abstraction;

public abstract class Entity : IEquatable<Entity>
{
    protected Entity(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; protected set; }

    public override bool Equals(object? obj)
    {
        return obj is Entity entity && Equals(entity);
    }

    public bool Equals(Entity? other)
    {
        return other is not null && Id == other.Id;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(GetType(), Id);
    }
}
