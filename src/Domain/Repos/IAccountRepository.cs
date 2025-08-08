using Domain.Entities;

namespace Domain.Repos;

public interface IAccountRepository : IRepository<Account>
{
    IEnumerable<Account> GetAll();

    Account? GetByIdWithData(Guid id);
}
