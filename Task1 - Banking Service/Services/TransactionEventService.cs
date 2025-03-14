using MediatR;
using Task1___Banking_Service.Models;
using Task1___Banking_Service.Repositories;

namespace Task1___Banking_Service.Services;

public class TransactionEventService
{
    private readonly IMediator _mediator;
    private readonly IUnitOfWork _unitOfWork;

    public TransactionEventService(IUnitOfWork unitOfWork,IMediator mediator)
    {
        _mediator = mediator;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<string> TransferFundsAsync(long fromAccountId, long toAccountId, decimal amount)
    {
        var fromAccount = await _unitOfWork.Accounts.GetByIdAsync(fromAccountId);
        var toAccount = await _unitOfWork.Accounts.GetByIdAsync(toAccountId);

        if (fromAccount == null || toAccount == null)
            return "Invalid account details.";

        if (fromAccount.Balance < amount)
            return "Insufficient funds.";

        fromAccount.Balance -= amount;
        toAccount.Balance += amount;

        var transaction = new TransactionLog
        {
            AccountId = fromAccountId,
            TransactionType = "Transfer",
            Amount = amount,
            Status = "Completed",
            Details = $"Transferred {amount} to account {toAccountId}"
        };

        await _unitOfWork.Transactions.AddTransactionAsync(transaction);
    
        // Ensure database changes are saved before publishing the event
        var changesSaved = await _unitOfWork.SaveChangesAsync() > 0;
        if (!changesSaved)
            return "Transaction failed.";

        await _mediator.Publish(new TransactionEvent
        {
            TransactionId = transaction.Id,
            EventType = "Transfer",
            Details = transaction.Details
        });

        return "Transfer successful.";
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