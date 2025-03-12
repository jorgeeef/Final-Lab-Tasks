using Microsoft.AspNetCore.Mvc;

namespace Task1___Banking_Service.Controllers;

[ApiController]
[Route("api/logs")]
public class LogsController : ControllerBase
{
    private readonly ILogger<LogsController> _logger;

    public LogsController(ILogger<LogsController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public IActionResult LogEvent([FromBody] string message)
    {
        _logger.LogInformation("New Event Logged: {Message}", message);
        return Ok(new { status = "Logged", message });
    }
}