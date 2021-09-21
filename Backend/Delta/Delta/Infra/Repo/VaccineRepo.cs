using Delta.Data;
using Delta.Infra.Interface;
using Delta.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delta.Infra.Repo
{
    public class VaccineRepo : IVaccine
    {
        private readonly DeltaContext _context;
        private readonly IMemoryCache _memoryCache;
        private readonly IDistributedCache _distributedCache;
        public VaccineRepo(
            DeltaContext context,
            IMemoryCache memoryCache,
            IDistributedCache distributedCache
            )
        {
            _context = context;
            _memoryCache = memoryCache;
            _distributedCache = distributedCache;
        }
 

        public async Task<IEnumerable<Vaccine>> SearchVaccine(string searchParams)
        {
            return await _context.Vaccine.Where(v => v.Name.Contains(searchParams) || v.Name.Contains(searchParams)).ToListAsync();
        }
    }
}
