namespace Task1___Banking_Service.Repositories;

public interface IUnitOfWork: IDisposable
{
    Task<int> SaveChangesAsync();
    ITransactionRepository Transactions { get; }
    IAccountRepository Accounts { get; }
}