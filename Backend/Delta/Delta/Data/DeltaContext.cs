using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Delta.Models;

namespace Delta.Data
{
    public class DeltaContext : DbContext
    {
        public DeltaContext (DbContextOptions<DeltaContext> options)
            : base(options)
        {
        }

        public DbSet<Delta.Models.Vaccine> Vaccine { get; set; }
    }
}
