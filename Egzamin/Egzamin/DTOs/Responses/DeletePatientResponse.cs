using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Egzamin.DTOs.Responses
{
    public class DeletePatientResponse
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Birthdate { get; set; }
    }
}
