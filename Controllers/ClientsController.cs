using CarRentalAPI.DTOs;
using CarRentalAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalAPI.Controllers;

/// <summary>
/// Kontroler klientów – "chudy": deleguje logikę do IClientService.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ClientsController : ControllerBase
{
    private readonly IClientService _clientService;

    public ClientsController(IClientService clientService) => _clientService = clientService;

    /// <summary>Zwraca listę aktywnych klientów.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ClientDto>), StatusCodes.Status200OK)]
    public IActionResult GetAll() => Ok(_clientService.GetAll());

    /// <summary>Zwraca klienta po Id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ClientDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(int id) => Ok(_clientService.GetById(id));

    /// <summary>
    /// Rejestruje nowego klienta.
    /// RB-2: klient musi mieć min. MinDriverAgeYears lat (z appsettings).
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ClientDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public IActionResult Create([FromBody] CreateClientDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var created = _clientService.Create(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>Aktualizuje dane klienta (imię, nazwisko, e-mail, telefon).</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ClientDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public IActionResult Update(int id, [FromBody] UpdateClientDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        return Ok(_clientService.Update(id, dto));
    }

    /// <summary>Soft-delete klienta. Klient z aktywnymi wypożyczeniami nie może być usunięty.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public IActionResult SoftDelete(int id, [FromBody] SoftDeleteDto dto)
    {
        _clientService.SoftDelete(id, dto);
        return NoContent();
    }
}
