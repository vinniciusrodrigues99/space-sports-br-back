using Microsoft.AspNetCore.Mvc;

namespace FSP.Api.WebApi.Controllers
{
    [ApiController]
    [Route("api/health")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        [HttpHead]
        public IActionResult Get() => Ok(new { status = "healthy", timestamp = DateTimeOffset.UtcNow });
    }
}
