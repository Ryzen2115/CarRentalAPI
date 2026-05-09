using CarRentalAPI.DTOs;
using CarRentalAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalAPI.Controllers;

/// <summary>
/// Kontroler samochodów – "chudy": tylko odbiera żądania HTTP,
/// deleguje całą logikę do ICarService.
/// Wyjątki obsługuje GlobalExceptionMiddleware (brak try/catch tutaj).
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CarsController : ControllerBase
{
    private readonly ICarService _carService;

    public CarsController(ICarService carService) => _carService = carService;

    /// <summary>Zwraca listę aktywnych samochodów (soft-deleted ukryte).</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CarDto>), StatusCodes.Status200OK)]
    public IActionResult GetAll() => Ok(_carService.GetAll());

    /// <summary>Zwraca pojedynczy samochód po Id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CarDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(int id) => Ok(_carService.GetById(id));

    /// <summary>Dodaje nowy samochód do floty.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(CarDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public IActionResult Create([FromBody] CreateCarDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var created = _carService.Create(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>Aktualizuje dane samochodu (nie można edytować wypożyczonego).</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(CarDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public IActionResult Update(int id, [FromBody] UpdateCarDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        return Ok(_carService.Update(id, dto));
    }

    /// <summary>
    /// Soft-delete samochodu (IsDeleted=true). Rekord pozostaje w bazie.
    /// Nie można usunąć samochodu aktualnie wypożyczonego.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public IActionResult SoftDelete(int id, [FromBody] SoftDeleteDto dto)
    {
        _carService.SoftDelete(id, dto);
        return NoContent();
    }
}
