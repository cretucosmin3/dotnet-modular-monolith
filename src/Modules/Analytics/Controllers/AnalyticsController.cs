using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AnalyticsController : ControllerBase
{
    [HttpGet("stats")]
    public IActionResult GetStats()
    {
        return Ok(100);
    }
}