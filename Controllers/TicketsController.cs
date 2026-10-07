using API_CINE.Dtos;
using API_CINE.Models;
using API_CINE.Services;
// USAMOS PARA USAR LOS METODOS DE ASPNETCORE PARA NUESTRO CONTROLADOR
using Microsoft.AspNetCore.Mvc;

namespace API_CINE.Controllers;

// CONTROLADOR  , EL QUE VA A RECIBIR LAS PETICIONES HTTP Y LLAMAR AL SERVICIO
[ApiController]
[Route("api/tickets")]

// HEREDARA LOS METODOS DEL CONTROLBASE
public class TicketsController : ControllerBase
{
    private ITicketService _service; // SOLO CONOCE EL INTERFAZ NO LA CLASE PA QUE SEA MAS FACIL CAMBIARLA SI QUEREMOS

    // INYECCION DE DEPENDENCIAS , PARA PODER USAR NUESTRO SERVICIO SIN INSTANCIAR LA CLASE YA NOS LO DA EL BUILDER EN PROGRAM.CS
    public TicketsController(ITicketService service)
    {
        _service = service;
    }

    //ActionResult para devolver datos de tipo Ticket o  lista de tickect
    [HttpGet]
    public ActionResult<List<Ticket>> ObtenerTodos()
    {
        return Ok(_service.ObtenerTodos()); // ok=200
    }

    [HttpGet("{id}")]
    public ActionResult<Ticket> ObtenerPorId(int id)
    {
        return Ok(_service.ObtenerPorId(id));
    }

    [HttpPost]

    public ActionResult<Ticket> Crear(TicketRequestDto dto)
    {
        Ticket creado = _service.Crear(dto);
        // devuelve el ticket creado y su ruta
        return CreatedAtAction("ObtenerPorId", new { id = creado.Id }, creado); // createdataction=201
    }

    [HttpPut("{id}")]
    public ActionResult<Ticket> Actualizar(int id, TicketRequestDto dto)
    {
        return Ok(_service.Actualizar(id, dto));
    }

    [HttpDelete("{id}")]
    public IActionResult Eliminar(int id)
    {
        _service.Eliminar(id);

        //retorna un 204 , se completo la soliti pero no hay nd que devolver
        return NoContent();
    }
}
