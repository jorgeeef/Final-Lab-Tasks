using Microsoft.EntityFrameworkCore;
using Task1___Banking_Service.Data;

namespace Banking_Service___Unit_Testing.TestHelpers;

public static class InMemoryDbContextFactory
{
    public static TransactionDbContext Create()
    {
        var options = new DbContextOptionsBuilder<TransactionDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new TransactionDbContext(options);
    }
}