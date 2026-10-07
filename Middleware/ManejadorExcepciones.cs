using API_CINE.Dtos;
using API_CINE.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace API_CINE.Middleware;

// MANEJADOR CENTRALIZADO DE ERRORES , TODA EXCEPCION QUE SE LANCE EN EL CONTROLADOR O SERVICIO LLEGA AQUI PARA NO PONER TRY/CATCH EN CADA METODO
// IMPLEMENTAMOS LA INTERFAZ IEXCEPTIONHANDLER
public class ManejadorExcepciones : IExceptionHandler
{
    private ILogger<ManejadorExcepciones> _logger; // PA ESCRIBIR LOS ERRORES EN  LA CONSOLA

    // INYECCION DE DEPENDENCIAS , ASP.NET NOS ENTREGA EL LOGGER SOLITO
    public ManejadorExcepciones(ILogger<ManejadorExcepciones> logger)
    {
        _logger = logger;
    }
    // ASYNC PORQUE PUEDE HABER MUCHOS ERRORES AL MISMO TIEMPO PARA NO BLOQUEAR EL CURSO
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        int estado;
        string error;
        string mensaje = exception.Message;

        // SEGUN EL TIPO DE EXCEPCION ELEGIMOS EL CODIGO HTTP
        switch (exception)
        {
            case NoEncontradoException:
                estado = StatusCodes.Status404NotFound;
                error = "No encontrado";
                break;
            case ReglaNegocioException:
                estado = StatusCodes.Status400BadRequest;
                error = "Solicitud inválida";
                break;
            case ConflictoException:
                estado = StatusCodes.Status409Conflict;
                error = "Conflicto";
                break;
            default:
                // POR SI HAY UN BUG , LO GUARDAMOS EN EL LOG PA NOSOTROS
                _logger.LogError(exception, "Error no controlado");
                estado = StatusCodes.Status500InternalServerError;
                error = "Error interno";
                mensaje = "Ocurrió un error inesperado. Intente más tarde.";
                break;
        }

        // ARMAMOS LA RESPUESTA CON NUESTRO DTO DE SALIDA
        ErrorResponse respuesta = new ErrorResponse
        {
            Estado = estado,
            Error = error,
            Mensaje = mensaje,
            Ruta = context.Request.Path
        };
        // LE PONEMOS EL CODIGO HTTP Y LA MANDAMOS COMO JSON
        context.Response.StatusCode = estado;
        await context.Response.WriteAsJsonAsync(respuesta, cancellationToken);

        return true;
    }
}
