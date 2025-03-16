using Microsoft.EntityFrameworkCore;
using Task1___Banking_Service.Data;
using Task1___Banking_Service.Models;

namespace Task1___Banking_Service.Services;

public class EventSourcingService
{
     private readonly TransactionDbContext _context;
     
     public EventSourcingService(TransactionDbContext context)
        {
            _context = context;
        }

        public async Task<bool> RollbackEventsAsync(DateTime date, long? accountId, string? eventType)
        {
            var events = _context.TransactionEvents
                .Where(e => e.Timestamp.Date == date.Date);

            if (accountId.HasValue)
                events = events.Where(e => e.TransactionId == accountId.Value);

            if (!string.IsNullOrEmpty(eventType))
                events = events.Where(e => e.EventType == eventType);

            var eventList = await events.ToListAsync();

            foreach (var transactionEvent in eventList)
            {
                var transaction = await _context.Transactions
                    .FirstOrDefaultAsync(t => t.Id == transactionEvent.TransactionId);

                if (transaction != null)
                {
                    if (transactionEvent.EventType == "Deposit")
                        transaction.Amount -= transaction.Amount;
                    else if (transactionEvent.EventType == "Withdrawal")
                        transaction.Amount += transaction.Amount;

                    _context.Transactions.Remove(transaction);
                }

                _context.TransactionEvents.Remove(transactionEvent);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<TransactionEvent>> GetEventsByFiltersAsync(long? accountId, string? eventType, DateTime? startDate, DateTime? endDate)
        {
            var query = _context.TransactionEvents.AsQueryable();

            if (accountId.HasValue)
                query = query.Where(e => e.TransactionId == accountId.Value);

            if (!string.IsNullOrEmpty(eventType))
                query = query.Where(e => e.EventType == eventType);

            if (startDate.HasValue)
                query = query.Where(e => e.Timestamp >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(e => e.Timestamp <= endDate.Value);

            return await query.ToListAsync();
        }
    
}