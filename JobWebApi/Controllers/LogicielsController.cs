using JobWebApi.Enities;
using JobWebApi.Services;
using Microsoft.AspNetCore.Mvc;


namespace JobWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogicielsController : ControllerBase
    {
        private readonly IServiceLogiciels _serviceLogi;
        private readonly ILogger<LogicielsController> _logger;

        public LogicielsController(IServiceLogiciels service, ILogger<LogicielsController> logger)
        {
            _serviceLogi = service;
            _logger = logger;
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

        // GET: Logiciels/GENOMICA/versions?millesime=2023
        [HttpGet("{codeLogiciel}/versions")]
        public async Task<ActionResult<IEnumerable<Version>>> GetVersions(string codeLogiciel, [FromQuery] int? millesime)
        {
            var versions = await _serviceLogi.ObtenirVersionsLogiciel(codeLogiciel, millesime);

            if (versions == null) return NotFound();

            return Ok(versions);
        }

        // GET : api/Logiciels/GENOMICA/Versions/1.00/Releases/30
        [HttpGet("{codeLogiciel}/Versions/{numVersion}/Releases/{numRelease}")]
        public async Task<ActionResult<IEnumerable<Version>>> GetRelease(string codeLogiciel, float numVersion, short numRelease)
        {
            var release = await _serviceLogi.ObtenirRelease(codeLogiciel, numVersion, numRelease);

            if (release == null) return NotFound();

            return Ok(release);
        }

        // POST : api/Logiciels/GENOMICA/Versions/1.00/Releases
        [HttpPost("{codeLogiciel}/Versions/{numVersion}/Releases")]
        public async Task<ActionResult<Release>> PostRelease(string codeLogiciel, float numVersion, [FromForm] FormRelease fr)
        {
            // Crée une entité du modèle à partir de l'entité DTO 
            // cela permet de transmettre le doonnées de FormRelease dans Release ordinnaires
            Release rel = new Release()
            {
                CodeLogiciel = codeLogiciel,
                NumeroVersion = numVersion,
                Numero = fr.Numero,
                DatePubli = fr.DatePubli
            };

            if (fr.Notes != null)
            {
                using StreamReader reader = new(fr.Notes.OpenReadStream());
                rel.Notes = await reader.ReadToEndAsync();
            }
            try
            {
                Release res = await _serviceLogi.AjouterRelease(codeLogiciel, numVersion, rel);

                object clé = new { codeLogiciel = res.CodeLogiciel, numVersion = res.NumeroVersion, numRelease = res.Numero };
                string uri = Url.Action(nameof(GetRelease), clé) ?? "";
                return Created(uri, res);
            }
            catch (Exception e)
            {
                return this.CustomResponseForError(e, rel, _logger);

            }
        }
    }
}
