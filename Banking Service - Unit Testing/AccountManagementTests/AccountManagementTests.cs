using Banking_Service___Unit_Testing.TestHelpers;
using Microsoft.EntityFrameworkCore;
using Moq;
using Task1___Banking_Service.Data;
using Task1___Banking_Service.Models;
using Task1___Banking_Service.Repositories;

namespace Banking_Service___Unit_Testing.AccountManagementTests;

public class AccountManagementTests
{
    private readonly TransactionDbContext _context;
    private readonly AccountRepository _accountRepository;

    public AccountManagementTests()
    {
        _context = InMemoryDbContextFactory.Create();
        _accountRepository = new AccountRepository(_context);
    }

    [Fact]
    public async Task CreateAccount_WithValidData_ShouldCreateAccount()
    {
        // Arrange
        var account = new Account
        {
            Balance = 1000,
            AccountStatus = "Active",
            NameTranslations = new Dictionary<string, string> { { "en", "Savings Account" } }
        };

        // Act
        await _context.Accounts.AddAsync(account);
        await _context.SaveChangesAsync();

        // Assert
        var savedAccount = await _accountRepository.GetByIdAsync(account.Id);
        Assert.NotNull(savedAccount);
        Assert.Equal(1000, savedAccount.Balance);
        Assert.Equal("Active", savedAccount.AccountStatus);
    }

    [Fact]
    public async Task GetAccount_NonExistingId_ShouldReturnNull()
    {
        // Act
        var result = await _accountRepository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAccount_ChangingStatus_ShouldUpdateSuccessfully()
    {
        // Arrange
        var account = new Account
        {
            Balance = 500,
            AccountStatus = "Active"
        };
        await _context.Accounts.AddAsync(account);
        await _context.SaveChangesAsync();

        // Act
        account.AccountStatus = "Inactive";
        await _accountRepository.UpdateAsync(account);

        // Assert
        var updatedAccount = await _accountRepository.GetByIdAsync(account.Id);
        Assert.Equal("Inactive", updatedAccount.AccountStatus);
    }
}