using MediatR;
using Task1___Banking_Service.Data;
using Task1___Banking_Service.Models;

namespace Task1___Banking_Service.Handler;

public class TransactionEventHandler: INotificationHandler<TransactionEvent>
{
    private readonly TransactionDbContext _context;

    public TransactionEventHandler(TransactionDbContext context)
    {
        _context = context;
    }

    public async Task Handle(TransactionEvent notification, CancellationToken cancellationToken)
    {
        // Logic to handle the event
        await _context.TransactionEvents.AddAsync(notification, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}