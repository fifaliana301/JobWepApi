using JobWebApi.Data;
using JobWebApi.Enities;
using Microsoft.EntityFrameworkCore;

namespace JobWebApi.Services
{
    public interface IServiceEquipes
    {
        Task<List<Equipe>> ObtenirEquipes(string codeFilière);
        Task<Equipe?> ObtenirEquipe(string codeFilière, string codeEquipe);
        Task<Equipe> AjouterEquipe(string codeFilière, Equipe équipe);
        Task<Personne> AjouterPersonne(string codeEquipe, Personne personne);
    }
    public class ServiceEquipes : IServiceEquipes
    {
        private readonly ContextJobWebApi _context;

        public ServiceEquipes(ContextJobWebApi context)
        {
            _context = context;
        }

        public async Task<List<Equipe>> ObtenirEquipes(string codeFilière)
        {
            var req = from e in _context.Equipes.Include(e => e.Service)
                      where e.CodeFiliere == codeFilière
                      select e;

            return await req.ToListAsync();
        }

        public async Task<Equipe?> ObtenirEquipe(string codeFilière, string codeEquipe)
        {
            var req = from e in _context.Equipes
                         .Include(e => e.Service)
                         .Include(e => e.Personnes)
                         .ThenInclude(p => p.Métier)
                      where e.Code == codeEquipe
                      select e;

            return await req.FirstOrDefaultAsync();
        }

        // Ajoute une équipe avec des personnes dans une filière donnée
        public async Task<Equipe> AjouterEquipe(string codeFilière, Equipe équipe)
        {
            équipe.CodeFiliere = codeFilière;
            équipe.Service = null!;
            foreach (Personne p in équipe.Personnes)
            {
                p.Métier = null!;
            }
            _context.Equipes.Add(équipe);

            await _context.SaveChangesAsync();

            return équipe;
        }

        // Ajoute une personne dans une équipe donnée
        public async Task<Personne> AjouterPersonne(string codeEquipe, Personne personne)
        {
            personne.CodeEquipe = codeEquipe;
            personne.Métier = null!;

            _context.Personnes.Add(personne);
            await _context.SaveChangesAsync();

            return personne;
        }
    }
}
