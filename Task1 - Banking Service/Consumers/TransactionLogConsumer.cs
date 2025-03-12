using Task1___Banking_Service.Data;
using Task1___Banking_Service.Models;

namespace Task1___Banking_Service.Consumers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
public class TransactionLogConsumer: BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private IModel _channel;
    private IConnection _connection;

    public TransactionLogConsumer(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
        var factory = new ConnectionFactory() { HostName = "localhost" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: "transaction_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var transactionLog = JsonConvert.DeserializeObject<TransactionLog>(message);

            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<TransactionDbContext>();
                context.TransactionLogs.Add(transactionLog);
                await context.SaveChangesAsync();
            }
        };
        _channel.BasicConsume(queue: "transaction_queue", autoAck: true, consumer: consumer);

        return Task.CompletedTask;
    }
}