using Domain.Abstraction;

namespace Domain.Repos;

public interface IRepository<TRoot>
    where TRoot : AggregateRoot
{
    TRoot? GetById(Guid id);

    void Insert(TRoot root);

    void Update(TRoot root);

    void Delete(TRoot root);
}
