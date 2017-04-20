using Microsoft.AspNetCore.Mvc;

namespace MyCodeCamp.Controllers
{
    [Route("api/[controller]")]
    public class CampsController : Controller
    {
        [HttpGet("")]
        public IActionResult Get()
        {
            return Ok(new {Name = "Shawn", FavoriteColor = "Blue"});
        }
    }
}