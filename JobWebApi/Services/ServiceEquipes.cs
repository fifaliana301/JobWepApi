using JobWebApi.Data;
using JobWebApi.Enities;
using Microsoft.EntityFrameworkCore;

namespace JobWebApi.Services
{
    public interface IServiceEquipes
    {
        Task<List<Equipe>> ObtenirEquipes(string codeFilière);
        Task<Equipe?> ObtenirEquipe(string codeFilière, string codeEquipe);
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
    }
}
