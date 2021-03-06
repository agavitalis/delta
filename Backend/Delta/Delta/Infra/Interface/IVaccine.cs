using Delta.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delta.Infra.Interface
{
    interface IVaccine
    {
        public Task<IEnumerable<Vaccine>> SearchVaccine(string searchParams);
    }
}
