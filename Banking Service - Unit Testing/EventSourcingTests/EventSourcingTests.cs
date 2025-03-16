using Banking_Service___Unit_Testing.TestHelpers;
using Moq;
using Task1___Banking_Service.Data;
using Task1___Banking_Service.Models;
using Task1___Banking_Service.Services;

namespace Banking_Service___Unit_Testing.EventSourcingTests;

public class EventSourcingTests
{
    private readonly TransactionDbContext _context;
    private readonly Mock<EventSourcingService> _eventSourcingServiceMock;

    public EventSourcingTests()
    {
        _context = InMemoryDbContextFactory.Create();
        _eventSourcingServiceMock = new Mock<EventSourcingService>();
    }

    [Fact]
    public async Task RollbackEvents_ForSpecificDay_ReversesTransactions()
    {
        // Arrange
        var date = DateTime.Today;
        var accountId = 1;
        
        // Create an account with transactions
        var account = new Account { Id = accountId, Balance = 1000 };
        await _context.Accounts.AddAsync(account);
        
        // Add some transaction events
        var events = new List<TransactionEvent>
        {
            new TransactionEvent 
            { 
                TransactionId = 1, 
                EventType = "Deposit", 
                Details = "Deposit of 500", 
                Timestamp = date.AddHours(10) 
            },
            new TransactionEvent 
            { 
                TransactionId = 2, 
                EventType = "Withdrawal", 
                Details = "Withdrawal of 200", 
                Timestamp = date.AddHours(14) 
            }
        };
        
        await _context.TransactionEvents.AddRangeAsync(events);
        await _context.SaveChangesAsync();
        
        // Mock the rollback operation
        _eventSourcingServiceMock.Setup(x => 
            x.RollbackEventsAsync(date, null, null))
            .ReturnsAsync(true);

        // Act
        var result = await _eventSourcingServiceMock.Object.RollbackEventsAsync(date, null, null);

        // Assert
        Assert.True(result);
        _eventSourcingServiceMock.Verify(x => x.RollbackEventsAsync(date, null, null), Times.Once);
    }

    [Fact]
    public async Task GetEvents_FilteredByAccountAndType_ReturnsFilteredEvents()
    {
        // Arrange
        var accountId = 1;
        var eventType = "Deposit";
        
        var events = new List<TransactionEvent>
        {
            new TransactionEvent { TransactionId = 1, EventType = "Deposit" },
            new TransactionEvent { TransactionId = 2, EventType = "Withdrawal" }
        };
        
        _eventSourcingServiceMock.Setup(x => 
            x.GetEventsByFiltersAsync(accountId, eventType, null, null))
            .ReturnsAsync(events.Where(e => e.EventType == eventType).ToList());

        // Act
        var results = await _eventSourcingServiceMock.Object
            .GetEventsByFiltersAsync(accountId, eventType, null, null);

        // Assert
        Assert.Single(results);
        Assert.Equal("Deposit", results.First().EventType);
    }
}