using System.Linq;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

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

        [HttpPost]
        public IActionResult Create([FromBody] CelestialObject celestialObject)
        {
            _context.Add(celestialObject);
            _context.SaveChanges();
            return CreatedAtRoute("GetById", new {id = celestialObject.Id }, celestialObject);

        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject celestialObject)
        {
            var celestialDb = _context.CelestialObjects.FirstOrDefault(x => x.Id == id);
            if (celestialDb == null)
                return NotFound();

            celestialDb.Name = celestialObject.Name;
            celestialDb.OrbitalPeriod = celestialObject.OrbitalPeriod;
            celestialDb.OrbitedObjectId = celestialObject.OrbitedObjectId;
            _context.Update(celestialDb);
            _context.SaveChanges();

            return NoContent();
        }
        
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var celestialDb = _context.CelestialObjects.Where(x => x.Id == id).ToList();
            if (!celestialDb.Any())
                return NotFound();
            
            _context.RemoveRange(celestialDb);
            _context.SaveChanges();

            return NoContent();
        }
        
        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var celestialDb = _context.CelestialObjects.FirstOrDefault(x => x.Id == id);
            if (celestialDb == null)
                return NotFound();

            celestialDb.Name = name;
            _context.Update(celestialDb);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
