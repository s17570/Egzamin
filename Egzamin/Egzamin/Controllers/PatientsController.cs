using Egzamin.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Egzamin.Controllers
{
    [Route("api/patients")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private IClinicDbService _service;

        public PatientsController(IClinicDbService service)
        {
            _service = service;
        }

        [HttpDelete("{id}")]
        public IActionResult DeletePatient(string id)
        {
            try
            {
                var result = _service.DeletePatient(id);

                if (result == null)
                {
                    return NotFound("Nie znaleziono pacjenta o podanym id.");
                }

                return Ok(result);

            }
            catch (Exception exception)
            {
                return BadRequest(exception.ToString());
            }
        }
    }
}
