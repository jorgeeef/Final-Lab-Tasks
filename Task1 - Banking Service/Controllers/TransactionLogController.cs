using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Task1___Banking_Service.Data;
using Task1___Banking_Service.Models;
using Task1___Banking_Service.Repositories;
using Task1___Banking_Service.Services;

namespace Task1___Banking_Service.Controllers;

[Route("api/transaction-logs")]
[ApiController]
public class TransactionLogController : ControllerBase
{
    private readonly TransactionDbContext _context;
    private readonly IModel _channel;
    private readonly TransactionEventService _transactionService;
    private readonly INotificationService _notificationService;

    public TransactionLogController(TransactionDbContext context, INotificationService notificationService,TransactionEventService transactionEventService, IModel channel)
    {
        _context = context;
        _channel = channel;
        _transactionService = transactionEventService;
        _notificationService = notificationService;
    }

    

    [HttpPost]
    public async Task<IActionResult> CreateTransactionLog([FromBody] TransactionLog log)
    {
        _context.TransactionLogs.Add(log);
        await _context.SaveChangesAsync();

        var message = JsonConvert.SerializeObject(log);
        var body = Encoding.UTF8.GetBytes(message);
        _channel.BasicPublish(exchange: "", routingKey: "transaction_queue", basicProperties: null, body: body);

        return CreatedAtAction(nameof(GetTransactionLogById), new { id = log.Id }, log);
    }

    // OData for filtering, sorting, and pagination
    [HttpGet]
    [EnableQuery]
    public IActionResult GetTransactionLogs()
    {
        return Ok(_context.TransactionLogs);
    }
    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetTransactionLogById(long id)
    {
        var log = await _context.TransactionLogs.FindAsync(id);
        if (log == null) return NotFound();
        return Ok(log);
    }

    [HttpGet("account/{accountId:long}")]
    public async Task<IActionResult> GetTransactionLogsByAccountId(long accountId)
    {
        var logs = await _context.TransactionLogs.Where(t => t.AccountId == accountId).ToListAsync();
        return Ok(logs);
    }
    
    
    // Task2: GET /accounts/common-transactions
        [HttpGet("common-transactions")]
        public async Task<IActionResult> GetCommonTransactions([FromQuery] List<long> accountIds)
        {
            if (accountIds == null || accountIds.Count < 2)
            {
                return BadRequest("Please provide at least two account IDs.");
            }

            var commonTransactions = await _context.TransactionLogs
                .Where(t => accountIds.Contains(t.AccountId))
                .ToListAsync();

            if (!commonTransactions.Any())
            {
                return NotFound("No common transactions found for the given accounts.");
            }

            // Group by transaction type and amount 
            var commonTransactionGroups = commonTransactions
                .GroupBy(t => new { t.TransactionType, t.Amount })
                .Where(g => g.Count() > 1)
                .Select(g => new
                {
                    TransactionId = g.FirstOrDefault().Id,
                    AccountIds = g.Select(t => t.AccountId).Distinct().ToList(),
                    g.Key.TransactionType,
                    g.Key.Amount
                })
                .ToList();

            return Ok(commonTransactionGroups);
        }

        // Task 2: GET /accounts/balance-summary/{userId}
        [HttpGet("balance-summary/{userId}")]
        public async Task<IActionResult> GetAccountBalanceSummary(long userId)
        {
            var accountTransactions = await _context.AccountTransaction
                .Where(at => at.UserId == userId)
                .ToListAsync();

            if (!accountTransactions.Any())
            {
                return NotFound("No accounts found for the given user.");
            }
            
            var transactions = await _context.TransactionLogs
                .Where(t => accountTransactions.Select(at => at.AccountId).Contains(t.AccountId))
                .ToListAsync();

            var balanceSummary = transactions
                .GroupBy(t => t.AccountId)
                .Select(g => new
                {
                    AccountId = g.Key,
                    TotalDeposits = g.Where(t => t.TransactionType == "Deposit").Sum(t => t.Amount),
                    TotalWithdrawals = g.Where(t => t.TransactionType == "Withdrawal").Sum(t => t.Amount),
                    TotalBalance = g.Where(t => t.TransactionType == "Deposit").Sum(t => t.Amount) -
                                   g.Where(t => t.TransactionType == "Withdrawal").Sum(t => t.Amount)
                })
                .ToList();

            return Ok(new { UserId = userId, Accounts = balanceSummary });
        }
        
        //Task5: Handles fund transfer between two accounts
        
        [HttpPost("transfer")]
        public async Task<IActionResult> TransferFunds([FromBody] long FromAccountId ,long ToAccountId ,decimal Amount)
        {
            if (FromAccountId == ToAccountId)
                return BadRequest("Cannot transfer to the same account.");

            await _transactionService.TransferFundsAsync(FromAccountId, ToAccountId, Amount);
            
            return Ok(new { Message = "Transfer successful." });
        }
        
        [ApiController]
        [Route("transactions")]
        public class TransactionController : ControllerBase
        {
            private readonly ITransactionRepository _transactionRepository;
            private readonly INotificationService _notificationService;

            // Inject both repositories through the constructor
            public TransactionController(ITransactionRepository transactionRepository, INotificationService notificationService)
            {
                _transactionRepository = transactionRepository;
                _notificationService = notificationService;
            }

            [HttpPost("notify")]
            public async Task<IActionResult> SendTransactionNotification(
                [FromBody] long transactionId, 
                [FromHeader(Name = "Accept-Language")] string language = "en")
            {
                
                var transaction = await _transactionRepository.GetByIdAsync(transactionId);
                if (transaction == null) 
                    return NotFound(new { Message = "Transaction not found" });

                // Fetch the message in the requested language or fall back to English
                string message = transaction.DescriptionTranslations != null &&
                                 transaction.DescriptionTranslations.ContainsKey(language)
                    ? transaction.DescriptionTranslations[language]
                    : transaction.DescriptionTranslations?["en"] ?? "Transaction details not available";

                // Send the notification
                await _notificationService.SendNotificationAsync($"Transaction Alert: {message}");

                return Ok(new { Message = "Notification sent successfully." });
            }
        }
    
}