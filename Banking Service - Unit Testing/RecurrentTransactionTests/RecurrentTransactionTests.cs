using Banking_Service___Unit_Testing.TestHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Moq;
using Task1___Banking_Service.Controllers;
using Task1___Banking_Service.Data;
using Task1___Banking_Service.Models;
using Task1___Banking_Service.Repositories;

namespace Banking_Service___Unit_Testing.RecurrentTransactionTests;

public class RecurrentTransactionTests
{
     private readonly Mock<ITransactionRepository> _mockTransactionRepository;
    private readonly Mock<IStringLocalizer<TransactionLogController.TransactionController>> _mockLocalizer;
    private readonly Mock<INotificationRepository> _mockNotificationRepository;
    private readonly TransactionDbContext _context; 
    private readonly TransactionLogController.TransactionController _controller;

    public RecurrentTransactionTests()
    {
        _mockTransactionRepository = new Mock<ITransactionRepository>();
        _mockLocalizer = new Mock<IStringLocalizer<TransactionLogController.TransactionController>>();
        _mockNotificationRepository = MockServices.GetMockNotificationService(); 
        _context = InMemoryDbContextFactory.Create();
    }

    [Fact]
    public async Task SendTransactionNotification_ValidTransaction_ReturnsOk()
    {
        // Arrange
        var transactionId = 1;

        // Create a sample transaction
        var transaction = new Transaction
        {
            Id = transactionId,
            Amount = 100,
        };

        // Set up mock to return the transaction when fetched by ID
        _mockTransactionRepository
            .Setup(repo => repo.GetByIdAsync(transactionId))
            .ReturnsAsync(transaction);

        // Set up localizer for missing transaction
        _mockLocalizer
            .Setup(localizer => localizer["TransactionNotFound"])
            .Returns(new LocalizedString("TransactionNotFound", "Transaction not found"));

        // Act
        var result = await _controller.SendTransactionNotification(transactionId); 

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        Assert.Equal("Transaction notification sent", ((dynamic)okResult.Value).Message);
    }
}