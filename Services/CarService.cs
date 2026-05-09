using CarRentalAPI.DTOs;
using CarRentalAPI.Exceptions;
using CarRentalAPI.Models;
using CarRentalAPI.Storage;

namespace CarRentalAPI.Services;

/// <summary>
/// Serwis samochodów – cała logika biznesowa i mapowanie Model ↔ DTO.
/// Kontroler jest "chudy": tylko wywołuje serwis i zwraca wynik.
/// Wyjątki domenowe są wyrzucane stąd i przechwytywane przez GlobalExceptionMiddleware.
/// </summary>
public class CarService : ICarService
{
    private readonly InMemoryStore _store;

    public CarService(InMemoryStore store) => _store = store;

    // ── GET all ──────────────────────────────────────────────────────────────
    public IEnumerable<CarDto> GetAll()
        => _store.ActiveCars.Select(MapToDto);

    // ── GET by id ────────────────────────────────────────────────────────────
    public CarDto GetById(int id)
    {
        var car = _store.ActiveCars.FirstOrDefault(c => c.Id == id)
            ?? throw new NotFoundException("Samochód", id);
        return MapToDto(car);
    }

    // ── POST ─────────────────────────────────────────────────────────────────
    public CarDto Create(CreateCarDto dto)
    {
        if (_store.ActiveCars.Any(c =>
                c.LicensePlate.Equals(dto.LicensePlate.Trim(), StringComparison.OrdinalIgnoreCase)))
            throw new DomainException(
                $"Samochód z numerem rejestracyjnym '{dto.LicensePlate}' już istnieje.");

        var car = new Car
        {
            Id           = _store.NextCarId(),
            Brand        = dto.Brand.Trim(),
            Model        = dto.Model.Trim(),
            Year         = dto.Year,
            LicensePlate = dto.LicensePlate.Trim().ToUpper(),
            Color        = dto.Color.Trim(),
            FuelType     = dto.FuelType.Trim(),
            DailyRate    = dto.DailyRate,
            IsAvailable  = true,
            CreatedAt    = DateTime.UtcNow
        };

        _store.Cars.Add(car);
        return MapToDto(car);
    }

    // ── PUT ──────────────────────────────────────────────────────────────────
    public CarDto Update(int id, UpdateCarDto dto)
    {
        var car = _store.ActiveCars.FirstOrDefault(c => c.Id == id)
            ?? throw new NotFoundException("Samochód", id);

        // Reguła: nie można edytować samochodu aktualnie wypożyczonego
        if (!car.IsAvailable)
            throw new DomainException(
                $"Nie można edytować samochodu '{car.Brand} {car.Model}', " +
                "który jest aktualnie wypożyczony.");

        car.Brand    = dto.Brand.Trim();
        car.Model    = dto.Model.Trim();
        car.Year     = dto.Year;
        car.Color    = dto.Color.Trim();
        car.FuelType = dto.FuelType.Trim();
        car.DailyRate = dto.DailyRate;

        return MapToDto(car);
    }

    // ── DELETE (soft) ────────────────────────────────────────────────────────
    public void SoftDelete(int id, SoftDeleteDto dto)
    {
        var car = _store.Cars.FirstOrDefault(c => c.Id == id)
            ?? throw new NotFoundException("Samochód", id);

        if (car.IsDeleted)
            throw new NotFoundException("Samochód", id);

        if (!car.IsAvailable)
            throw new DomainException(
                "Nie można usunąć samochodu, który jest aktualnie wypożyczony.");

        car.IsDeleted = true;
        car.DeletedAt = DateTime.UtcNow;
        car.DeletedBy = dto.DeletedBy;
    }

    // ── Mapping ──────────────────────────────────────────────────────────────
    private static CarDto MapToDto(Car c) => new()
    {
        Id           = c.Id,
        Brand        = c.Brand,
        Model        = c.Model,
        Year         = c.Year,
        LicensePlate = c.LicensePlate,
        Color        = c.Color,
        FuelType     = c.FuelType,
        DailyRate    = c.DailyRate,
        IsAvailable  = c.IsAvailable,
        CreatedAt    = c.CreatedAt
    };
}
