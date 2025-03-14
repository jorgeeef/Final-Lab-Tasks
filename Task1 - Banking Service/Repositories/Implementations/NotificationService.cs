namespace Task1___Banking_Service.Repositories;

public class NotificationService: INotificationService
{
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(ILogger<NotificationService> logger)
    {
        _logger = logger;
    }

    public async Task SendNotificationAsync(string message)
    {
        _logger.LogInformation($"Sending notification: {message}");
        await Task.CompletedTask; 
    }
}