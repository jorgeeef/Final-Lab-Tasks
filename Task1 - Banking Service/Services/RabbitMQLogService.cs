using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using Task1___Banking_Service.Data;
using Task1___Banking_Service.Models;

namespace Task1___Banking_Service.Services;

public class RabbitMQLogService: BackgroundService
{
    private readonly TransactionDbContext _context;
    private readonly string _queueName = "logQueue";
    private readonly string _rabbitMQHost = "localhost";

    public RabbitMQLogService(TransactionDbContext context)
    {
        _context = context;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
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

        return Task.CompletedTask;
    }

    private void ProcessLogMessage(string message)
    {
        var log = JsonConvert.DeserializeObject<Logs>(message);

        if (log != null)
        {
            _context.Logs.Add(log);
            _context.SaveChanges();
        }
    }
}
