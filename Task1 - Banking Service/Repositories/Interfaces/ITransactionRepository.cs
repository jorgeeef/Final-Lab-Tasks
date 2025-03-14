using Task1___Banking_Service.Models;

namespace Task1___Banking_Service.Repositories;

public interface ITransactionRepository
{
    Task AddTransactionAsync(TransactionLog transaction);
}