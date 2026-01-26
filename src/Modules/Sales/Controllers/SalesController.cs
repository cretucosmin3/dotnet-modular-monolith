using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class SalesController : ControllerBase
{
    [HttpGet("monthly")]
    public IActionResult GetMonthly()
    {
        return Ok(100);
    }
}