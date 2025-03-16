namespace Task1___Banking_Service.Repositories;

public interface INotificationRepository
{
    Task SendNotificationAsync(string message);
}