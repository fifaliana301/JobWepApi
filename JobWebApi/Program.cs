
using JobWebApi.Data;
using JobWebApi.Services;
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

            builder.Services.AddScoped<IServiceLogiciels, ServiceLogiciels>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            object value = builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
