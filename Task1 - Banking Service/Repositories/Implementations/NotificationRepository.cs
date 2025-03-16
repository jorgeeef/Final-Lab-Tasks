namespace Task1___Banking_Service.Repositories;

public class NotificationRepository: INotificationRepository
{
    private readonly ILogger<NotificationRepository> _logger;

    public NotificationRepository(ILogger<NotificationRepository> logger)
    {
        _logger = logger;
    }

    public async Task SendNotificationAsync(string message)
    {
        _logger.LogInformation($"Sending notification: {message}");
        await Task.CompletedTask; 
    }
}