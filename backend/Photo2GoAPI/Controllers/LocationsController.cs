using Microsoft.AspNetCore.Mvc;
using Photo2GoAPI.Data; 
using System.Linq;

namespace Photo2GoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LocationsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LocationsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("filter")]
        public IActionResult FilterLocations([FromQuery] string? objectType)
        {
            var locations = _context.Locations.AsQueryable();

            if (!string.IsNullOrEmpty(objectType))
            {
                locations = locations.Where(l => l.ObjectType == objectType);
            }

            return Ok(locations.ToList());
        }
    }
}