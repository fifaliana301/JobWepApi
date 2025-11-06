using JobWebApi.Data;
using JobWebApi.Enities;
using Microsoft.EntityFrameworkCore;

namespace JobWebApi.Services
{
    public interface IServiceLogiciels
    {
        Task<List<Logiciel>> ObtenirLogiciels();
        Task<Logiciel?> ObtenirLogiciel(string code);
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
    }
}
