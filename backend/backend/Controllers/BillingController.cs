using Microsoft.AspNetCore.Mvc;

namespace VPBackend_Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BillingController : ControllerBase
    {
        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            return Ok("Billing service is running.");
        }
    }
} 