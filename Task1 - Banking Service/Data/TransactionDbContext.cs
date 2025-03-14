using Microsoft.EntityFrameworkCore;
using Task1___Banking_Service.Models;

namespace Task1___Banking_Service.Data;

public class TransactionDbContext: DbContext
{
    public TransactionDbContext(DbContextOptions<TransactionDbContext> options) : base(options) { }

    public DbSet<TransactionLog> TransactionLogs { get; set; }
    public DbSet<AccountTransaction> AccountTransaction { get; set; }
    
    public DbSet<Account> Accounts { get; set; }
    public DbSet<TransactionEvent> TransactionEvents { get; set; }
    
    public DbSet<Transaction> Transactions { get; set; }
    
    public DbSet<Logs> Logs { get; set; } 
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Logs>()
            .HasIndex(log => log.RequestId)
            .HasDatabaseName("IX_RequestId");

        modelBuilder.Entity<Logs>()
            .HasIndex(log => log.Timestamp)
            .HasDatabaseName("IX_Timestamp");
    }
    
}