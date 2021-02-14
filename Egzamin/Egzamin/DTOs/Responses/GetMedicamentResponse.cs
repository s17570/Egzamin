using Egzamin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Egzamin.DTOs.Responses
{
    public class GetMedicamentResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public List<Prescription> Prescriptions { get; set; }
    }
}
