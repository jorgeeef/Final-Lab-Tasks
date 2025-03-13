using Microsoft.AspNetCore.Mvc;
using Task1___Banking_Service.Services;

namespace Task1___Banking_Service.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EventsController : ControllerBase
{
    private readonly TransactionEventService _eventService;

    public EventsController(TransactionEventService eventService)
    {
        _eventService = eventService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateEvent([FromBody] TransactionEventRequest request)
    {
        await _eventService.DispatchEvent(request.TransactionId, request.EventType, request.Details);

        return Ok(new { message = "Event created and dispatched successfully." });
    }
}

public class TransactionEventRequest
{
    public long TransactionId { get; set; }
    public string EventType { get; set; }
    public string Details { get; set; }
}