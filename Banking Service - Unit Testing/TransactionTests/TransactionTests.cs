using Banking_Service___Unit_Testing.TestHelpers;
using MediatR;
using Moq;
using Task1___Banking_Service.Data;
using Task1___Banking_Service.Models;
using Task1___Banking_Service.Repositories;
using Task1___Banking_Service.Services;

namespace Banking_Service___Unit_Testing.TransactionTests;

public class TransactionTests
{
     private readonly TransactionDbContext _context;
    private readonly AccountRepository _accountRepository;
    private readonly TransactionRepository _transactionRepository;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly TransactionEventService _transactionService;
    private readonly IUnitOfWork _unitOfWork;

    public TransactionTests()
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
    public async Task TransferFunds_ValidAmount_UpdatesBalances()
    {
        // Arrange
        var fromAccount = new Account { Balance = 1000, AccountStatus = "Active" };
        var toAccount = new Account { Balance = 500, AccountStatus = "Active" };
        await _context.Accounts.AddRangeAsync(fromAccount, toAccount);
        await _context.SaveChangesAsync();

        // Act
        var result = await _transactionService.TransferFundsAsync(fromAccount.Id, toAccount.Id, 300);

        // Assert
        Assert.Equal("Transfer successful.", result);
        
        // Verify balances updated
        var updatedFromAccount = await _accountRepository.GetByIdAsync(fromAccount.Id);
        var updatedToAccount = await _accountRepository.GetByIdAsync(toAccount.Id);
        
        Assert.Equal(700, updatedFromAccount.Balance);
        Assert.Equal(800, updatedToAccount.Balance);
        
        // Verify event published
        _mediatorMock.Verify(m => m.Publish(It.IsAny<TransactionEvent>(), default), Times.Once);
    }

    [Fact]
    public async Task TransferFunds_InsufficientBalance_ReturnsError()
    {
        // Arrange
        var fromAccount = new Account { Balance = 100, AccountStatus = "Active" };
        var toAccount = new Account { Balance = 500, AccountStatus = "Active" };
        await _context.Accounts.AddRangeAsync(fromAccount, toAccount);
        await _context.SaveChangesAsync();

        // Act
        var result = await _transactionService.TransferFundsAsync(fromAccount.Id, toAccount.Id, 300);

        // Assert
        Assert.Equal("Insufficient funds.", result);
        
        // Verify no changes to balances
        var updatedFromAccount = await _accountRepository.GetByIdAsync(fromAccount.Id);
        var updatedToAccount = await _accountRepository.GetByIdAsync(toAccount.Id);
        
        Assert.Equal(100, updatedFromAccount.Balance);
        Assert.Equal(500, updatedToAccount.Balance);
        
        // Verify no event published
        _mediatorMock.Verify(m => m.Publish(It.IsAny<TransactionEvent>(), default), Times.Never);
    }

    [Fact]
    public async Task TransferFunds_NonExistentAccount_ReturnsError()
    {
        // Arrange
        var fromAccount = new Account { Balance = 1000, AccountStatus = "Active" };
        await _context.Accounts.AddAsync(fromAccount);
        await _context.SaveChangesAsync();

        // Act
        var result = await _transactionService.TransferFundsAsync(fromAccount.Id, 999, 300);

        // Assert
        Assert.Equal("Invalid account details.", result);
    }
}