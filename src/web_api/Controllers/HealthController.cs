using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace BackEnd.src.web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting("FixedWindowLimiter")]
    public class HealthController : ControllerBase
    {

        [HttpGet]
        [EnableRateLimiting("FixedWindowLimiter")]
        public IActionResult CheckConnection(){
            return Ok(new {message = "Connection is OK"});
        }
    }
}