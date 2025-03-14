using Task1___Banking_Service.Data;
using Task1___Banking_Service.Models;

namespace Task1___Banking_Service.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly TransactionDbContext _context;

    public AccountRepository(TransactionDbContext context)
    {
        _context = context;
    }

    public async Task<Account> GetByIdAsync(long accountId)
    {
        return await _context.Accounts.FindAsync(accountId);
    }

    public async Task UpdateAsync(Account account)
    {
        _context.Accounts.Update(account);
        await _context.SaveChangesAsync();
    }
}