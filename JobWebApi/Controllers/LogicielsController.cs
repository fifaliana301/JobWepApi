using JobWebApi.Data;
using JobWebApi.Enities;
using JobWebApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogicielsController : ControllerBase
    {
        private readonly IServiceLogiciels _serviceLogi;

        public LogicielsController(IServiceLogiciels service)
        {
            _serviceLogi = service;
        }

        // GET: api/Logiciels
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Logiciel>>> GetLogiciels()
        {
            return await _serviceLogi.ObtenirLogiciels();
        }

        // GET: api/Logiciels/5
        [HttpGet("{code}")]
        public async Task<ActionResult<Logiciel>> GetLogiciel(string code)
        {
            var logiciel = await _serviceLogi.ObtenirLogiciel(code);

            if (logiciel == null)
            {
                return NotFound();
            }

            return logiciel;
        }
    }
}
