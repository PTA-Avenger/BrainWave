using Microsoft.AspNetCore.Mvc;

namespace BrainWave.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
	[HttpGet]
	public IActionResult Get() => Ok(new { status = "ok", timeUtc = DateTime.UtcNow });
}
