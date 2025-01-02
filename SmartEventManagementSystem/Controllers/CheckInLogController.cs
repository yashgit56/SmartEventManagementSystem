using Microsoft.AspNetCore.Mvc;
using Smart_Event_Management_System.Dto;
using Smart_Event_Management_System.Models;
using Smart_Event_Management_System.Service;

namespace Smart_Event_Management_System.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CheckInLogController : ControllerBase
{
    private readonly ICheckInLogService _service;

    public CheckInLogController(ICheckInLogService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CheckInLog>>> GetAllCheckInLogs()
    {
        var checkInLogs = await _service.GetAllCheckInLogsAsync();
        return Ok(checkInLogs);
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<CheckInLog>> GetCheckInLogById(int id)
    {
        var checkInLog = await _service.GetCheckInLogByIdAsync(id);

        if (checkInLog == null) return NotFound();

        return Ok(checkInLog);
    }


    [HttpPost]
    public async Task<ActionResult<CheckInLog>> CreateCheckInLog([FromBody] CheckInLogDto checkInLogDto)
    {
        if (checkInLogDto == null) return BadRequest("CheckInLogDto data is required");

        var checkInLog = new CheckInLog(checkInLogDto.EventId, checkInLogDto.AttendeeId);

        var createdCheckInLog = await _service.CreateCheckInLogAsync(checkInLog);

        return CreatedAtAction(
            nameof(GetCheckInLogById),
            new { id = createdCheckInLog.Id },
            createdCheckInLog
        );
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCheckInLog(int id)
    {
        var result = await _service.DeleteCheckInLogAsync(id);

        if (!result) return NotFound();

        return NoContent();
    }
}