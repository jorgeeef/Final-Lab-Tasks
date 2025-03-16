using Moq;
using Task1___Banking_Service.Repositories;

namespace Banking_Service___Unit_Testing.TestHelpers;

public static class MockServices
{
    public static Mock<INotificationService> GetMockNotificationService()
    {
        var mockService = new Mock<INotificationService>();
        mockService.Setup(s => s.SendNotificationAsync(It.IsAny<string>()))
            .Returns(Task.CompletedTask);
        return mockService;
    }
}