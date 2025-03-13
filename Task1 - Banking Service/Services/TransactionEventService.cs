using MediatR;
using Task1___Banking_Service.Models;

namespace Task1___Banking_Service.Services;

public class TransactionEventService
{
    private readonly IMediator _mediator;

    public TransactionEventService(IMediator mediator)
    {
        _mediator = mediator;
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