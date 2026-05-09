using System.Net;
using System.Text.Json;
using CarRentalAPI.Exceptions;

namespace CarRentalAPI.Middleware;

/// <summary>
/// Globalny middleware do obsługi wyjątków.
/// Przechwytuje wszystkie nieobsłużone wyjątki i zamienia je
/// na ujednolicone odpowiedzi JSON z odpowiednim kodem HTTP.
/// Dzięki temu kontrolery są "chude" – nie zawierają try/catch.
/// </summary>
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger)
    {
        _next   = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Nieobsłużony wyjątek: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, title) = exception switch
        {
            NotFoundException   => (HttpStatusCode.NotFound,            "Zasób nie został znaleziony"),
            DomainException     => (HttpStatusCode.Conflict,            "Naruszenie reguły biznesowej"),
            ValidationException => (HttpStatusCode.BadRequest,          "Błąd walidacji"),
            ArgumentException   => (HttpStatusCode.BadRequest,          "Nieprawidłowe dane wejściowe"),
            _                   => (HttpStatusCode.InternalServerError, "Wewnętrzny błąd serwera")
        };

        var response = new
        {
            status    = (int)statusCode,
            title,
            message   = exception.Message,
            timestamp = DateTime.UtcNow
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode  = (int)statusCode;

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}
