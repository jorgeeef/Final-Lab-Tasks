using MediatR;
using Task1___Banking_Service.Models;
using Task1___Banking_Service.Repositories;

namespace Task1___Banking_Service.Services;

public class TransactionEventService
{
    private readonly IMediator _mediator;
    private readonly IUnitOfWork _unitOfWork;

    public TransactionEventService(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    public async Task CreateTransaction(long transactionId, string eventType, string details)
    {
        var transactionEvent = new TransactionEvent
        {
            TransactionId = transactionId,
            EventType = eventType,
            Details = details,
            Timestamp = DateTime.UtcNow
        };

        await _mediator.Publish(transactionEvent);
    }

    public async Task DispatchEvent(long transactionId, string eventType, string details)
    {
        var transactionEvent = new TransactionEvent
        {
            TransactionId = transactionId,
            EventType = eventType,
            Details = details,
            Timestamp = DateTime.UtcNow
        };
        
        await _mediator.Publish(transactionEvent);
    }
}