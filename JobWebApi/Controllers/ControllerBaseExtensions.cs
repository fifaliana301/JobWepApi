using JobWebApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace JobWebApi.Controllers
{
    public static class ControllerBaseExtensions
    {
        // ActionResult est un type de retour utilisé dans les contrôleurs ASP.NET Core.
        // Permet de renvoyer différents types de réponses HTTP 
        public static ActionResult CustomResponseForError(this ControllerBase controller, Exception e)
        {
            if (e is DbUpdateException dbe)
            {
                ProblemDetails pb = dbe.ConvertToProblemDetails();
                return controller.Problem(pb.Detail, null, pb.Status, pb.Title);
            }
            else throw e;
        }

        // Journalise une erreur avec le détail de l'action et de l'entité concernées,
        // puis renvoie une réponse HTTP personnalisée
        public static ActionResult CustomResponseForError<T>(this ControllerBase controller,
            Exception e, T entity, ILogger logger, [CallerMemberName] string? action = null)
        {
            if (e is DbUpdateException dbe)
            {
                ProblemDetails pb = dbe.ConvertToProblemDetails();
                logger.LogWarning("Action {action}, entité de type {type}\n{détail}\n{entity}",
                    action,
                    entity?.GetType().Name,
                    pb.Detail,
                    JsonSerializer.Serialize(entity, new JsonSerializerOptions { WriteIndented = true }));

                return controller.Problem(pb.Detail, null, pb.Status, pb.Title);
            }
            else throw e;
        }
    }
}
