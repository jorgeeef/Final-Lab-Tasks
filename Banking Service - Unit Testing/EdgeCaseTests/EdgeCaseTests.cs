using Banking_Service___Unit_Testing.TestHelpers;
using MediatR;
using Moq;
using Task1___Banking_Service.Data;
using Task1___Banking_Service.Models;
using Task1___Banking_Service.Repositories;
using Task1___Banking_Service.Services;

namespace Banking_Service___Unit_Testing.EdgeCaseTests;

public class EdgeCaseTests
{
    private readonly TransactionDbContext _context;
    private readonly AccountRepository _accountRepository;
    private readonly TransactionRepository _transactionRepository;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly TransactionEventService _transactionService;
    private readonly IUnitOfWork _unitOfWork;

    public EdgeCaseTests()
    {
        _context = InMemoryDbContextFactory.Create();
        _accountRepository = new AccountRepository(_context);
        _transactionRepository = new TransactionRepository(_context);
        _mediatorMock = new Mock<IMediator>();
        
        // Create mock UnitOfWork
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(uow => uow.Accounts).Returns(_accountRepository);
        unitOfWorkMock.Setup(uow => uow.Transactions).Returns(_transactionRepository);
        unitOfWorkMock.Setup(uow => uow.SaveChangesAsync()).ReturnsAsync(1);
        _unitOfWork = unitOfWorkMock.Object;
        
        _transactionService = new TransactionEventService(_unitOfWork, _mediatorMock.Object);
    }

    [Fact]
    public async Task TransferFunds_ZeroAmount_ReturnsError()
    {
        // Arrange
        var fromAccount = new Account { Balance = 1000 };
        var toAccount = new Account { Balance = 500 };
        
        await _context.Accounts.AddRangeAsync(fromAccount, toAccount);
        await _context.SaveChangesAsync();

        // Act
        var result = await _transactionService.TransferFundsAsync(fromAccount.Id, toAccount.Id, 0);

        // Assert
        Assert.Contains("invalid", result.ToLower());
    }

    [Fact]
    public async Task TransferFunds_NegativeAmount_ReturnsError()
    {
        // Arrange
        var fromAccount = new Account { Balance = 1000 };
        var toAccount = new Account { Balance = 500 };
        
        await _context.Accounts.AddRangeAsync(fromAccount, toAccount);
        await _context.SaveChangesAsync();

        // Act
        var result = await _transactionService.TransferFundsAsync(fromAccount.Id, toAccount.Id, -100);

        // Assert
        Assert.Contains("invalid", result.ToLower());
    }

    [Fact]
    public async Task TransferFunds_MaximumAmount_ProcessesCorrectly()
    {
        // Arrange
        decimal maxAmount = decimal.MaxValue / 2; // To avoid overflow when adding
        var fromAccount = new Account { Balance = maxAmount };
        var toAccount = new Account { Balance = 0 };
        
        await _context.Accounts.AddRangeAsync(fromAccount, toAccount);
        await _context.SaveChangesAsync();

        // Act
        var result = await _transactionService.TransferFundsAsync(fromAccount.Id, toAccount.Id, maxAmount);

        // Assert
        Assert.Equal("Transfer successful.", result);
        
        var updatedFromAccount = await _accountRepository.GetByIdAsync(fromAccount.Id);
        var updatedToAccount = await _accountRepository.GetByIdAsync(toAccount.Id);
        
        Assert.Equal(0, updatedFromAccount.Balance);
        Assert.Equal(maxAmount, updatedToAccount.Balance);
    }

    [Fact]
    public async Task TransferFunds_ToSameAccount_ReturnsError()
    {
        // Arrange
        var account = new Account { Balance = 1000 };
        await _context.Accounts.AddAsync(account);
        await _context.SaveChangesAsync();

        // Act
        var result = await _transactionService.TransferFundsAsync(account.Id, account.Id, 100);

        // Assert
        Assert.Contains("invalid", result.ToLower());
    }

    [Fact]
    public async Task TransferFunds_AccountInactive_ReturnsError()
    {
        // Arrange
        var fromAccount = new Account { Balance = 1000, AccountStatus = "Inactive" };
        var toAccount = new Account { Balance = 500, AccountStatus = "Active" };
        
        await _context.Accounts.AddRangeAsync(fromAccount, toAccount);
        await _context.SaveChangesAsync();

        // Act
        var result = await _transactionService.TransferFundsAsync(fromAccount.Id, toAccount.Id, 100);

        // Assert
        Assert.Contains("inactive", result.ToLower());
    }
}