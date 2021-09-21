using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delta.Models
{
    [Index(nameof(Name), nameof(Variant))]
    public class Vaccine
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Variant { get; set; }
        public DateTime CreatedAt => DateTime.Now;
    }
}
