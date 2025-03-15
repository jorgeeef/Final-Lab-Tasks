using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Task1___Banking_Service.Data;
using Task1___Banking_Service.Models;

namespace Task1___Banking_Service.Controllers;

[ApiController]
[Route("api/logs")]
public class LogsController : ControllerBase
{
    private readonly ILogger<LogsController> _logger;
    private readonly TransactionDbContext _context;

    public LogsController(ILogger<LogsController> logger,TransactionDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpPost("log-event")]
    public IActionResult LogEvent([FromBody] string message)
    {
        _logger.LogInformation("New Event Logged: {Message}", message);
        return Ok(new { status = "Logged", message });
    }
    
    [HttpGet]
    public async Task<IActionResult> GetLogs([FromQuery] string requestId, [FromQuery] string routeUrl, [FromQuery] string startDate, [FromQuery] string endDate, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var query = _context.Logs.AsQueryable();

        if (!string.IsNullOrEmpty(requestId))
            query = query.Where(log => log.RequestId.ToString().Contains(requestId));

        if (!string.IsNullOrEmpty(routeUrl))
            query = query.Where(log => log.RouteURL.Contains(routeUrl));

        if (DateTime.TryParse(startDate, out var start) && DateTime.TryParse(endDate, out var end))
            query = query.Where(log => log.Timestamp >= start && log.Timestamp <= end);

        // Apply pagination
        var logs = await query
            .OrderBy(log => log.Timestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(logs);
    }

    
    [HttpPost("add-log")]
    public async Task<IActionResult> PostLog([FromBody] Logs log)
    {
        if (log == null)
            return BadRequest("Log cannot be null");

        _context.Logs.Add(log);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetLogs), new { id = log.Id }, log);
    }

}