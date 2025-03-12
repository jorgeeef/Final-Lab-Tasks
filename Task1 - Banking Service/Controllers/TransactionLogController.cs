using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task1___Banking_Service.Data;
using Task1___Banking_Service.Models;

namespace Task1___Banking_Service.Controllers;

[Route("api/transaction-logs")]
[ApiController]
public class TransactionLogController: ControllerBase
{
    private readonly TransactionDbContext _context;

    public TransactionLogController(TransactionDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTransactionLog([FromBody] TransactionLog log)
    {
        _context.TransactionLogs.Add(log);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetTransactionLogById), new { id = log.Id }, log);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetTransactionLogById(long id)
    {
        var log = await _context.TransactionLogs.FindAsync(id);
        
        if (log == null)
        {
            return NotFound();
        }
        
        return Ok(log);
    }

    [HttpGet("account/{accountId:long}")]
    public async Task<IActionResult> GetTransactionLogsByAccountId(long accountId)
    {
        var logs = await _context.TransactionLogs.Where(t => t.AccountId == accountId).ToListAsync();
        return Ok(logs);
    }
}