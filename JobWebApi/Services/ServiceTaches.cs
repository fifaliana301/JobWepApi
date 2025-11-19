using Microsoft.EntityFrameworkCore;
using JobWebApi.Data;
using JobWebApi.Enities;

namespace JobWebApi.Services
{
    public interface IServiceTaches
    {
        Task<List<Tache>> ObtenirTaches(string? personne, string? logiciel, float? version);
        Task<Tache?> ObtenirTache(int id);
        Task<Personne?> ObtenirPersonne(string peudo);
        Task<Tache> AjouterTache(Tache tache);
        Task<Travail> AjouterTravail(int idTache, Travail tache);
        Task SupprimerTravail(int idTache, DateTime date);
        Task<int> SupprimerTaches(string? personne, string? logiciel, float? version);
    }

    public class ServiceTaches : IServiceTaches
    {
        private readonly ContextJobWebApi _contexte;

        public ServiceTaches(ContextJobWebApi contexte)
        {
            _contexte = contexte;
        }

        // Renvoie le tâches pour une personne, un logiciel et une version donnés
        public async Task<List<Tache>> ObtenirTaches(string? personne, string? logiciel, float? version)
        {
            var req = from t in _contexte.Taches
                      where (personne == null || t.Personne == personne) &&
                             (logiciel == null || t.CodeLogiciel == logiciel) &&
                             (version == null || t.NumVersion == version)
                      orderby t.CodeLogiciel, t.NumVersion
                      select t;

            return await req.ToListAsync();
        }

        // Renvoie une tâche et ses travaux associés
        public async Task<Tache?> ObtenirTache(int id)
        {
            var req = from t in _contexte.Taches
                         .Include(x => x.Travaux.OrderBy(x => x.DateTravail))
                      where t.Id == id
                      select t;

            return await req.FirstOrDefaultAsync();
        }

        // Renvoie une personne avec son métier et ses activités ou null si le pseudo n'existe pas
        public async Task<Personne?> ObtenirPersonne(string pseudo)
        {
            var req = from p in _contexte.Personnes
                         .Include(p => p.Métier)
                         .ThenInclude(m => m.Activités)
                      where p.Pseudo == pseudo
                      select p;

            var pers = await req.FirstOrDefaultAsync();
            return pers;
        }

        // Ajoute une tâche
        public async Task<Tache> AjouterTache(Tache tache)
        {
            tache.Travaux = null!;

            _contexte.Taches.Add(tache);
            await _contexte.SaveChangesAsync();

            return tache;
        }

        // Ajoute un travail sur une tâche donnée
        public async Task<Travail> AjouterTravail(int idTache, Travail travail)
        {
            ValidationRulesException vre = new();
            if (travail.DateTravail.TimeOfDay != new TimeSpan())
                vre.Errors.Add("Date", new string[] { "La partie horaire de la date doit être à 0" });

            if (travail.Heures < 0.5m || travail.Heures > 8)
                vre.Errors.Add("Heures", new string[] { "Le nombre d'heures doit être compris entre 0.5 et 8" });

            if (vre.Errors.Any()) throw vre;

            // Récupère la tâche
            Tache? tache = await _contexte.Taches.FindAsync(idTache);
            //Tache? tache = await _contexte.Taches.Where(t => t.Id == idTache)
            //					.AsTracking().FirstOrDefaultAsync();
            if (tache == null)
                throw new ValidationRulesException("IdTache", $"Tache {idTache} non trouvée");

            // Récupère la personne associée à la tâche et ses activités
            Personne? p = await ObtenirPersonne(tache.Personne);

            travail.IdTache = idTache;
            travail.TauxProductivite = p!.TauxProductivite;

            // Met à jour la durée de travail restante sur la tâche
            tache.DureeRestante -= travail.Heures;
            if (tache.DureeRestante < 0) tache.DureeRestante = 0;

            _contexte.Travaux.Add(travail);
            await _contexte.SaveChangesAsync();

            return travail;
        }

        // Supprime un travail
        public async Task SupprimerTravail(int idTache, DateTime date)
        {
            // Récupère la tâche et ses travaux
            Tache? tache = await ObtenirTache(idTache);
            if (tache == null)
                throw new ValidationRulesException("IdTache", $"Tache {idTache} non trouvée");

            // Recherche le travail à supprimer
            Travail? travail = tache.Travaux.Where(t => t.DateTravail == date).FirstOrDefault();
            if (travail == null)
                throw new ValidationRulesException("IdTache", $"Aucun travail trouvé à la date du {date} sur la tâche {idTache}.");

            // Met à jour la durée de travail restante sur la tâche
            tache.DureeRestante += travail.Heures;

            // Supprime le travail
            _contexte.Remove(travail);

            await _contexte.SaveChangesAsync();
        }
        // Supprime les tâches correspondant au filtre
        // et leurs travaux associés par cascade
        public async Task<int> SupprimerTaches(string? personne, string? logiciel, float? version)
        {
            var req = _contexte.Taches.Where(t =>
                        (personne == null || t.Personne == personne) &&
                        (logiciel == null || t.CodeLogiciel == logiciel) &&
                        (version == null || t.NumVersion == version));

            return await req.ExecuteDeleteAsync();
        }
    }
}