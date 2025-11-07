using JobWebApi.Enities;
using JobWebApi.Services;
using Microsoft.AspNetCore.Mvc;


namespace JobWebApi.Controllers
{
    [Route("api/Filieres/{codeFiliere}/[controller]")]
    [ApiController]
    public class EquipesController : ControllerBase
    {
        private readonly IServiceEquipes _service;

        public EquipesController(IServiceEquipes service)
        {
            _service = service;
        }

        // GET: api/Filieres/BIOH/Equipes
        [HttpGet]
        public async Task<ActionResult<List<Equipe>>> GetEquipes(string codeFiliere)
        {
            var équipes = await _service.ObtenirEquipes(codeFiliere);
            if (!équipes.Any()) return NotFound();
            return Ok(équipes);
        }

        // GET: api/Filieres/BIOH/Equipes/BIOH_DEV
        [HttpGet("{codeEquipe}")]
        public async Task<ActionResult<Equipe?>> GetEquipe(string codeFiliere, string codeEquipe)
        {
            var équipe = await _service.ObtenirEquipe(codeFiliere, codeEquipe);
            if (équipe == null) return NotFound();
            return Ok(équipe);
        }

        // POST: api/Filieres/BIOV/Equipes
        [HttpPost]
        public async Task<ActionResult<Equipe>> PostEquipe(string codeFiliere, Equipe eq)
        {
            Equipe res = await _service.AjouterEquipe(codeFiliere, eq);

            // Renvoie une réponse de code 201 avec l'en-tête 
            // "location: <url d'accès à l’équipe>" et un corps contenant l’équipe
            return CreatedAtAction(nameof(GetEquipe), new { codeFiliere = res.CodeFiliere, codeEquipe = res.Code }, res); ;
        }

        // POST: api/Filieres/BIOV/Equipes/BIOV_MKT
        [HttpPost("{codeEquipe}")]
        public async Task<ActionResult<Equipe>> PostPersonne(string codeFiliere, string codeEquipe, Personne pers)
        {
            Personne res = await _service.AjouterPersonne(codeEquipe, pers);

            // Renvoie une réponse de code 201 avec l'en-tête 
            // "location: <url d'accès à l’équipe de la personne>" et un corps contenant l’équipe
            return CreatedAtAction(nameof(GetEquipe), new { codeFiliere, codeEquipe }, res);
        }
    }
}
