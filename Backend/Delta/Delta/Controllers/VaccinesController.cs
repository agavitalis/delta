using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Delta.Data;
using Delta.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using Newtonsoft.Json;
using Delta.Infra.Interface;

namespace Delta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VaccinesController : ControllerBase
    {
        private readonly DeltaContext _context;
        private readonly IMemoryCache _memoryCache;
        private readonly IDistributedCache _distributedCache;
        private readonly IVaccine _vaccineRepo;
        public VaccinesController(
            DeltaContext context,
            IMemoryCache memoryCache,
            IDistributedCache distributedCache,
            IVaccine vaccineRepo
            )
        {
            _context = context;
            _memoryCache = memoryCache;
            _distributedCache = distributedCache;
            _vaccineRepo = vaccineRepo;
        }

        // GET: api/Vaccines
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Vaccine>>> GetVaccine()
        {
            return await _context.Vaccine.ToListAsync();
        }

        // GET: api/Vaccines/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Vaccine>> GetVaccine(int id)
        {
            var vaccine = await _context.Vaccine.FindAsync(id);

            if (vaccine == null)
            {
                return NotFound();
            }

            return vaccine;
        }

        // GET: api/searchParams?searchParams
        [Route("SearchVaccine")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Vaccine>>>SearchVaccine(string searchParams)
        {
            
            var checkCache = _memoryCache.TryGetValue(searchParams, out List<Vaccine> vaccines);
            if (checkCache)
            {
                return Ok(vaccines);
            }
            vaccines = (List<Vaccine>)await _vaccineRepo.SearchVaccine(searchParams);
           // vaccines = await _context.Vaccine.Where(v=>v.Name.Contains(searchParams) || v.Name.Contains(searchParams)).ToListAsync();
            //add it to cache
            var cacheOptions = new MemoryCacheEntryOptions
            {
                Priority = CacheItemPriority.High,
                AbsoluteExpiration = DateTime.Now.AddSeconds(5),
                SlidingExpiration = TimeSpan.FromSeconds(4)
            };

            _memoryCache.Set(searchParams, vaccines, cacheOptions);

            return Ok(vaccines);
        }


        // GET: api/searchParams?searchParams
        [Route("SearchVaccineWithRaddis")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Vaccine>>> SearchVaccineWithRaddis(string searchParams)
        {
            var vaccinesList = new List<Vaccine>();
            string serializedVaccineList;
            var vaccinesInCache = await _distributedCache.GetAsync(searchParams);

            if (vaccinesInCache != null)
            {
                serializedVaccineList = Encoding.UTF8.GetString(vaccinesInCache);
                vaccinesList = JsonConvert.DeserializeObject<List<Vaccine>>(serializedVaccineList);
                return Ok(vaccinesList);
            }

            vaccinesList = await _context.Vaccine.Where(v => v.Name.Contains(searchParams) || v.Name.Contains(searchParams)).ToListAsync();
            //add it to cache
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddSeconds(5),
                SlidingExpiration = TimeSpan.FromSeconds(4)
            };

            var serializedCustomerList = JsonConvert.SerializeObject(vaccinesList);
            var byteCustomerList = Encoding.UTF8.GetBytes(serializedCustomerList);

            _distributedCache.Set(searchParams, byteCustomerList, cacheOptions);

            return Ok(vaccinesList);
        }

        // PUT: api/Vaccines/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVaccine(int id, Vaccine vaccine)
        {
            if (id != vaccine.Id)
            {
                return BadRequest();
            }

            _context.Entry(vaccine).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VaccineExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Vaccines
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Vaccine>> PostVaccine(Vaccine vaccine)
        {
            _context.Vaccine.Add(vaccine);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetVaccine", new { id = vaccine.Id }, vaccine);
        }

        // DELETE: api/Vaccines/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVaccine(int id)
        {
            var vaccine = await _context.Vaccine.FindAsync(id);
            if (vaccine == null)
            {
                return NotFound();
            }

            _context.Vaccine.Remove(vaccine);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool VaccineExists(int id)
        {
            return _context.Vaccine.Any(e => e.Id == id);
        }
    }
}
