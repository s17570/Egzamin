using Egzamin.Models;
using Egzamin.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Egzamin.Controllers
{
    [Route("api/medicaments")]
    [ApiController]
    public class MedicamentsController : ControllerBase
    {
        private IClinicDbService _service;

        public MedicamentsController(IClinicDbService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public IActionResult GetModel(string id)
        {
            try
            {
                var result = _service.GetMedicament(id);

                if (result == null)
                {
                    return NotFound("Nie znaleziono leku o podanym id.");
                }

                return Ok(result);

            } catch (Exception exception)
            {
                return BadRequest(exception.ToString());
            }
        }
    }
}
