using Moq;
using Task1___Banking_Service.Repositories;

namespace Banking_Service___Unit_Testing.TestHelpers;

public static class MockServices
{
    public static Mock<INotificationRepository> GetMockNotificationService()
    {
        var mockService = new Mock<INotificationRepository>();
        mockService.Setup(s => s.SendNotificationAsync(It.IsAny<string>()))
            .Returns(Task.CompletedTask);
        return mockService;
    }
}