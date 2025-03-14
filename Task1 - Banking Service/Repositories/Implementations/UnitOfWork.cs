using Task1___Banking_Service.Data;

namespace Task1___Banking_Service.Repositories;

public class UnitOfWork: IUnitOfWork
{
    private readonly TransactionDbContext _context;
    public ITransactionRepository Transactions { get; }
    public IAccountRepository Accounts { get; }

    public UnitOfWork(TransactionDbContext context, ITransactionRepository transactions, IAccountRepository accounts)
    {
        _context = context;
        Transactions = transactions;
        Accounts = accounts;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}