using JobWebApi.Data;
using JobWebApi.Enities;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace JobWebApi.Services
{
    public interface IServiceLogiciels
    {
        Task<List<Logiciel>> ObtenirLogiciels();
        Task<Logiciel?> ObtenirLogiciel(string code);

        Task<List<Versions>?> ObtenirVersionsLogiciel(string codeLogiciel, int? millésime);
        Task<Release?> ObtenirRelease(string codeLogiciel, float numVersion, short numRelease);
        Task<Release> AjouterRelease(string codeLogiciel, float numVersion, Release release);

        Task<Versions> AjouterVersion(string codeLogiciel, Versions version);
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

            // Récupère le N° de release maxi pour le logiciel et la version
            var req1 = from r in _contexte.Releases
                       where r.CodeLogiciel == codeLogiciel && r.NumeroVersion == numVersion
                       orderby r.Numero
                       select r.Numero;

            short relMax = await req1.LastOrDefaultAsync(); // renvoie 0 s'il n'y a aucune release

            if (relMax > 0)
            {
                if (release.Numero <= relMax)
                {
                    ValidationRulesException ex = new();
                    ex.Errors.Add("Numero", new string[] { $"Le N° de release doit être > {relMax}" });
                    throw ex;
                }

                // Récupère la date de publication de la release précédente
                var req2 = from r in _contexte.Releases
                           where r.CodeLogiciel == codeLogiciel && r.NumeroVersion == numVersion && r.Numero == relMax
                           select r.DatePubli;
                DateTime datePubliPrec = await req2.FirstOrDefaultAsync();

                if (release.DatePubli < datePubliPrec)
                {
                    ValidationRulesException ex = new();
                    ex.Errors.Add("DatePubli", new string[] { $"La date de publication de la release doit être >= {datePubliPrec}" });
                    throw ex;
                }
            }

            _contexte.Releases.Add(release);
            await _contexte.SaveChangesAsync();

            return release;
        }

        public async Task<Versions> AjouterVersion(string codeLogiciel, Versions version)
        {
            version.CodeLogiciel = codeLogiciel;

            // Règles de validation
            ValidationRulesException ex = new();
            Regex regex = new Regex(@"^\d{1,3}(.\d{1,2})?$");
            if (!regex.IsMatch(version.Numero.ToString()))
                ex.Errors.Add("Numero", new string[] { $"Le numéro de version ({version.Numero}) doit avoir au maximum 3 chiffres avant la virgule et 2 après." });

            if (version.Millesime < 2020 || version.Millesime > 2100)
                ex.Errors.Add("Millesime", new string[] { $"Le millésime ({version.Millesime}) doit être compris entre 2020 et 2100 inclus" });

            if (version.DateOuverture >= version.DateSortiePrevue)
                ex.Errors.Add("DateOuverture", new string[] { $"La date d'ouverture doit être < à la date de sortie prévue." });

            if (version.DateSortieReelle != null && version.DateOuverture >= version.DateSortieReelle)
                ex.Errors.Add("DateOuverture", new string[] { $"La date d'ouverture doit être < à la date de sortie réelle." });

            if (ex.Errors.Any()) throw ex;

            // Enregistrement en base
            _contexte.Versions.Add(version);
            await _contexte.SaveChangesAsync();

            return version;
        }
    }
}
