using G2rismBeta.API.Models;
using System.Net;
using System.Text.Json;

namespace G2rismBeta.API.Middleware;

/// <summary>
/// Middleware global para capturar y manejar todas las excepciones
/// </summary>
public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public GlobalExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlerMiddleware> logger,
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    /// <summary>
    /// Método que se ejecuta en cada request
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Continuar con el siguiente middleware
            await _next(context);
        }
        catch (Exception ex)
        {
            // Si hay un error, capturarlo aquí
            await HandleExceptionAsync(context, ex);
        }
    }

    /// <summary>
    /// Maneja la excepción y devuelve una respuesta formateada
    /// </summary>
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // Log del error
        _logger.LogError(exception, "Ocurrió un error no controlado: {Message}", exception.Message);

        // Configurar respuesta HTTP
        context.Response.ContentType = "application/json";

        // Determinar el código de estado según el tipo de excepción
        var statusCode = exception switch
        {
            ArgumentException => HttpStatusCode.BadRequest,        // 400
            KeyNotFoundException => HttpStatusCode.NotFound,       // 404
            InvalidOperationException => HttpStatusCode.BadRequest,  // 400 (reglas de negocio violadas)
            UnauthorizedAccessException => HttpStatusCode.Unauthorized, // 401
            _ => HttpStatusCode.InternalServerError                // 500
        };

        context.Response.StatusCode = (int)statusCode;

        // Crear respuesta de error
        var errorResponse = new ApiErrorResponse
        {
            Success = false,
            Message = GetUserFriendlyMessage(exception),
            StatusCode = (int)statusCode,
            ErrorCode = exception.GetType().Name,
            Timestamp = DateTime.Now
        };

        // En desarrollo, incluir el StackTrace formateado para debugging
        if (_environment.IsDevelopment())
        {
            errorResponse.StackTrace = FormatStackTrace(exception);
        }

        // Serializar y enviar respuesta
        var json = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });

        await context.Response.WriteAsync(json);
    }

    /// <summary>
    /// Obtiene un mensaje amigable según el tipo de excepción
    /// </summary>
    private string GetUserFriendlyMessage(Exception exception)
    {
        return exception switch
        {
            ArgumentException => exception.Message,
            KeyNotFoundException => exception.Message,
            InvalidOperationException => exception.Message,
            UnauthorizedAccessException => "No tiene permisos para realizar esta acción",
            _ => "Ocurrió un error inesperado. Por favor, intente nuevamente más tarde."
        };
    }

    /// <summary>
    /// Formatea el StackTrace para mostrar solo las líneas más relevantes del código del usuario
    /// Filtra las llamadas internas del framework (Microsoft, System, etc.)
    /// </summary>
    private string FormatStackTrace(Exception exception)
    {
        if (string.IsNullOrEmpty(exception.StackTrace))
            return "No hay información de StackTrace disponible";

        var lines = exception.StackTrace.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        var relevantLines = new List<string>();
        var lineNumber = 1;

        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();

            // Filtrar líneas del framework y mostrar solo el código del usuario
            if (trimmedLine.Contains("G2rismBeta.API"))
            {
                // Esta es una línea de nuestro código - IMPORTANTE
                relevantLines.Add($"  [{lineNumber}] 🔴 {trimmedLine}");
                lineNumber++;
            }
            else if (trimmedLine.Contains("Microsoft.AspNetCore") ||
                     trimmedLine.Contains("Microsoft.EntityFrameworkCore"))
            {
                // Líneas del framework - opcional, mostrar de forma compacta
                var simplifiedLine = SimplifyFrameworkLine(trimmedLine);
                if (!string.IsNullOrEmpty(simplifiedLine))
                {
                    relevantLines.Add($"  [{lineNumber}] ⚪ {simplifiedLine}");
                    lineNumber++;
                }
            }
        }

        if (relevantLines.Count == 0)
        {
            return "StackTrace completo:\n" + exception.StackTrace;
        }

        var header = "📋 TRAZA DE EJECUCIÓN DEL ERROR:\n" +
                     "🔴 = Tu código (G2rismBeta.API)\n" +
                     "⚪ = Framework (ASP.NET Core / EF Core)\n\n";

        return header + string.Join("\n", relevantLines);
    }

    /// <summary>
    /// Simplifica las líneas del framework para hacerlas más legibles
    /// </summary>
    private string SimplifyFrameworkLine(string line)
    {
        // Extraer solo el nombre del método relevante
        if (line.Contains("Microsoft.AspNetCore.Mvc"))
        {
            if (line.Contains("ControllerActionInvoker"))
                return "ASP.NET Core: Ejecutando acción del controlador";
            if (line.Contains("ActionMethodExecutor"))
                return "ASP.NET Core: Ejecutando método del controlador";
        }

        if (line.Contains("Microsoft.EntityFrameworkCore"))
        {
            return "Entity Framework: Ejecutando consulta a base de datos";
        }

        if (line.Contains("AuthorizationMiddleware"))
        {
            return "ASP.NET Core: Verificando autorización";
        }

        if (line.Contains("SwaggerMiddleware") || line.Contains("SwaggerUI"))
        {
            return "Swagger: Procesando solicitud de documentación";
        }

        if (line.Contains("GlobalExceptionHandlerMiddleware"))
        {
            return "Middleware: Capturando y procesando la excepción";
        }

        // Si no es relevante, no mostrar
        return string.Empty;
    }
}
