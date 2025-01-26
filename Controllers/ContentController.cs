using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace DJPortfolio.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContentController : ControllerBase
    {
        [HttpGet("data")]
        public async Task<IActionResult> GetContent()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "content.json");
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Content file not found.");
            }

            var json = await System.IO.File.ReadAllTextAsync(filePath);
            var content = JsonSerializer.Deserialize<object>(json);
            return Ok(content);
        }
    }
}
