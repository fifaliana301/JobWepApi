
using JobWebApi.Data;
using Microsoft.EntityFrameworkCore;

namespace JobWebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            // R�cupere la chaine de connexion de la base dans les parametres
            string? connect = builder.Configuration.GetConnectionString("JobWebApiConnect");

            // Enregistre ton DbContext dans le conteneur DI
            builder.Services.AddDbContext<ContextJobWebApi>(options =>
                options.UseSqlServer(connect));

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
