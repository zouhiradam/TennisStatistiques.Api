using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace TennisStatistiques.Api.Exceptions
{
    public class GlobalExceptionHandler(
        IProblemDetailsService problemDetailsService,
        ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var (status, title) = exception switch
            {
                NoPlayersException => (StatusCodes.Status404NotFound, "Not Found"),
                _ => (StatusCodes.Status500InternalServerError, "Erreur interne du serveur")
            };

            if (status == StatusCodes.Status500InternalServerError)
                logger.LogError(exception, "Erreur non gérée");

            httpContext.Response.StatusCode = status;

            return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails = new ProblemDetails
                {
                    Status = status,
                    Title = title,
                    // On n'expose jamais le message d'une erreur inattendue au client
                    Detail = status == StatusCodes.Status500InternalServerError
                        ? "Une erreur inattendue est survenue."
                        : exception.Message
                }
            });
        }
    }
}
