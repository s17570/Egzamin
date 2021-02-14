using Egzamin.DTOs.Responses;
using Egzamin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Egzamin.Services
{
    public interface IClinicDbService
    {
        GetMedicamentResponse GetMedicament(string id);
        DeletePatientResponse DeletePatient(string id);
    }
}
