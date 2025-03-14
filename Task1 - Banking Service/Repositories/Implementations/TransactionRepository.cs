using Task1___Banking_Service.Data;
using Task1___Banking_Service.Models;

namespace Task1___Banking_Service.Repositories;

public class TransactionRepository: ITransactionRepository
{
    private readonly TransactionDbContext _context;

    public TransactionRepository(TransactionDbContext context)
    {
        _context = context;
    }

    public async Task AddTransactionAsync(TransactionLog transaction)
    {
        await _context.TransactionLogs.AddAsync(transaction);
        await _context.SaveChangesAsync();
    }
    
    public async Task<Transaction> GetByIdAsync(long id)
    {
        return await _context.Transactions.FindAsync(id);
    }
}