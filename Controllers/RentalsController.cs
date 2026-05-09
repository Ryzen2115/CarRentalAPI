using CarRentalAPI.DTOs;
using CarRentalAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalAPI.Controllers;

/// <summary>
/// Kontroler wypożyczeń – "chudy": deleguje logikę do IRentalService.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class RentalsController : ControllerBase
{
    private readonly IRentalService _rentalService;

    public RentalsController(IRentalService rentalService) => _rentalService = rentalService;

    /// <summary>Zwraca listę wszystkich wypożyczeń.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<RentalDto>), StatusCodes.Status200OK)]
    public IActionResult GetAll() => Ok(_rentalService.GetAll());

    /// <summary>Zwraca szczegóły wypożyczenia po Id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(RentalDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(int id) => Ok(_rentalService.GetById(id));

    /// <summary>
    /// Tworzy nowe wypożyczenie.
    /// RB-1: max MaxActiveRentalsPerClient na klienta.
    /// RB-3: samochód musi być dostępny.
    /// RB-4: max MaxRentalDays dni.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(RentalDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public IActionResult Create([FromBody] CreateRentalDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var created = _rentalService.Create(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
}
