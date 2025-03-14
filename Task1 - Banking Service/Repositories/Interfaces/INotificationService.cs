namespace Task1___Banking_Service.Repositories;

public interface INotificationService
{
    Task SendNotificationAsync(string message);
}