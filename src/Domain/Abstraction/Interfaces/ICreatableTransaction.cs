using Domain.Entities;
using Domain.Shared;
using Domain.ValueObjects;

namespace Domain.Abstraction.Interfaces;

public interface ICreatableTransaction
{
    static abstract Result<Transaction> Create(Money amount, DateTime date, string category, string description);
}
