using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Task1___Banking_Service.Data;
using Task1___Banking_Service.Models;

namespace Task1___Banking_Service.Consumers;

public class TransactionLogConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IModel _channel;

    public TransactionLogConsumer(IServiceScopeFactory scopeFactory, IModel channel)
    {
        _scopeFactory = scopeFactory;
        _channel = channel;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var transactionLog = JsonConvert.DeserializeObject<TransactionLog>(message);

            if (transactionLog != null)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<TransactionDbContext>();
                    context.TransactionLogs.Add(transactionLog);
                    await context.SaveChangesAsync(stoppingToken);
                }
            }
        };

        _channel.BasicConsume(queue: "transaction_queue", autoAck: true, consumer: consumer);

        return Task.CompletedTask;
    }
}