using System.Linq;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase 
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            this._context = context;
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            if (_context.CelestialObjects.All(x => x.Id != id))
                return NotFound();
            var celestial = _context.CelestialObjects.First(_ => _.Id == id);
            celestial.Satellites = _context.CelestialObjects.Where(_ => _.OrbitedObjectId == celestial.Id).ToList();
            return Ok(celestial);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            if (_context.CelestialObjects.All(_ => _.Name != name))
                return NotFound();
            
            var celestials = _context.CelestialObjects.Where(_ => _.Name == name).ToList();
            celestials.ForEach(x=>x.Satellites = _context.CelestialObjects.Where(_=>_.OrbitedObjectId == x.Id).ToList());
            return Ok(celestials);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var celestials = _context.CelestialObjects.ToList();
            celestials.ForEach(x=>x.Satellites = celestials.Where(_=>_.OrbitedObjectId == x.Id).ToList());
            return Ok(celestials);
        }
    }
}
