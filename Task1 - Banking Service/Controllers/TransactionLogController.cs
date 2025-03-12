using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Task1___Banking_Service.Data;
using Task1___Banking_Service.Models;

namespace Task1___Banking_Service.Controllers;

[Route("api/transaction-logs")]
[ApiController]
public class TransactionLogController : ControllerBase
{
    private readonly TransactionDbContext _context;
    private readonly IModel _channel;

    public TransactionLogController(TransactionDbContext context, IModel channel)
    {
        _context = context;
        _channel = channel;
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
}