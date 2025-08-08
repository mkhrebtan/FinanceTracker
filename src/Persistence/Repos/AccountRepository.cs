using Domain.Entities;
using Domain.Repos;
using Domain.ValueObjects;

namespace Persistence.Repos;

public class AccountRepository : GenericRepository<Account>, IAccountRepository
{
    public AccountRepository()
    {
        Account account1 = Account.Create(Money.Zero("USD").Value).Value;
        Account account2 = Account.Create(Money.Create(200, "USD").Value).Value;

        _roots.TryAdd(account1.Id, account1);
        _roots.TryAdd(account2.Id, account2);
    }

    public IEnumerable<Account> GetAll()
    {
        return _roots.Values;
    }

    public Account? GetByIdWithData(Guid id)
    {
        throw new NotImplementedException();
    }
}
