using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Task1___Banking_Service.Data;
using Task1___Banking_Service.Models;

public class RabbitMQLogService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly string _queueName = "logQueue";
    private readonly string _rabbitMQHost = "localhost";

    public RabbitMQLogService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory() { HostName = _rabbitMQHost };
        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();
        channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (sender, e) =>
        {
            var body = e.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            ProcessLogMessage(message);
        };

        channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

        await Task.CompletedTask;
    }

    private void ProcessLogMessage(string message)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<TransactionDbContext>();
            var log = JsonConvert.DeserializeObject<Logs>(message);

            if (log != null)
            {
                context.Logs.Add(log);
                context.SaveChanges();
            }
        }
    }
}