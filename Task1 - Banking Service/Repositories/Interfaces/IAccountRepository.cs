using Task1___Banking_Service.Models;

namespace Task1___Banking_Service.Repositories;

public interface IAccountRepository
{
    Task<Account> GetByIdAsync(long accountId);
    Task UpdateAsync(Account account);
}