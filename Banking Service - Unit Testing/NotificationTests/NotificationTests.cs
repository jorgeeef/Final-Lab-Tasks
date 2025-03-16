using Banking_Service___Unit_Testing.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using Task1___Banking_Service.Repositories;

namespace Banking_Service___Unit_Testing.NotificationTests;

public class NotificationTests
{
    private readonly Mock<ILogger<NotificationRepository>> _loggerMock;
    private readonly NotificationRepository _notificationRepository;
    private readonly Mock<INotificationRepository> _mockNotificationRepository;

    public NotificationTests()
    {
        _loggerMock = new Mock<ILogger<NotificationRepository>>();
        _notificationRepository = new NotificationRepository(_loggerMock.Object);
        _mockNotificationRepository = MockServices.GetMockNotificationService();
    }

    [Fact]
    public async Task SendNotification_ValidMessage_LogsInformation()
    {
        // Arrange
        string message = "Test notification";

        // Act
        await _notificationRepository.SendNotificationAsync(message);

        // Assert
        // Verify logging happened
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString().Contains(message)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task MockNotificationService_SendsNotification_CompletesTask()
    {
        // Arrange
        string message = "Test notification";

        // Act
        await _mockNotificationRepository.Object.SendNotificationAsync(message);

        // Assert
        _mockNotificationRepository.Verify(x => x.SendNotificationAsync(message), Times.Once);
    }
}