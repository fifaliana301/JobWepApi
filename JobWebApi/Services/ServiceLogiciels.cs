using JobWebApi.Data;
using JobWebApi.Enities;
using Microsoft.EntityFrameworkCore;

namespace JobWebApi.Services
{
    public interface IServiceLogiciels
    {
        Task<List<Logiciel>> ObtenirLogiciels();
        Task<Logiciel?> ObtenirLogiciel(string code);

        Task<List<Versions>?> ObtenirVersionsLogiciel(string codeLogiciel, int? millésime);
        Task<Release?> ObtenirRelease(string codeLogiciel, float numVersion, short numRelease);
        Task<Release> AjouterRelease(string codeLogiciel, float numVersion, Release release);
    }
    public class ServiceLogiciels : IServiceLogiciels
    {
        private readonly ContextJobWebApi _contexte;

        public ServiceLogiciels(ContextJobWebApi contexte)
        {
            _contexte = contexte;
        }

        public async Task<List<Logiciel>> ObtenirLogiciels()
        {
            return await _contexte.Logiciels.ToListAsync();
        }

        public async Task<Logiciel?> ObtenirLogiciel(string code)
        {
            return await _contexte.Logiciels.FindAsync(code);
        }

        // Versions et releases d'un logiciel
        public async Task<List<Versions>?> ObtenirVersionsLogiciel(string codeLogiciel, int? millésime)
        {
            // On vérifie si le logiciel existe
            if (await _contexte.Logiciels.FindAsync(codeLogiciel) == null)
                return null;

            // On récupère ses versions et releases
            var req = from v in _contexte.Versions.Include(v => v.Releases)
                      where v.CodeLogiciel == codeLogiciel &&
                              (millésime == null || v.Millesime == millésime)
                      select v;

            return await req.ToListAsync();
        }

        public async Task<Release?> ObtenirRelease(string codeLogiciel, float numVersion, short numRelease)
        {
            return await _contexte.Releases.FindAsync(numRelease, numVersion, codeLogiciel);
        }

        public async Task<Release> AjouterRelease(string codeLogiciel, float numVersion, Release release)
        {
            release.CodeLogiciel = codeLogiciel;
            release.NumeroVersion = numVersion;

            _contexte.Releases.Add(release);
            await _contexte.SaveChangesAsync();

            return release;
        }
    }
}
