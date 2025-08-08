using Domain.Abstraction;
using Domain.Repos;
using System.Collections.Concurrent;

namespace Persistence.Repos;

public class GenericRepository<TRoot> : IRepository<TRoot>
    where TRoot : AggregateRoot
{
    protected readonly ConcurrentDictionary<Guid, TRoot> _roots = [];

    public void Delete(TRoot root)
    {
        _roots.TryRemove(root.Id, out _);
    }

    public TRoot? GetById(Guid id)
    {
        return _roots.TryGetValue(id, out var root) ? root : null;
    }

    public void Insert(TRoot root)
    {
        _roots.TryAdd(root.Id, root);
    }

    public void Update(TRoot root)
    {
        throw new NotImplementedException();
    }
}
