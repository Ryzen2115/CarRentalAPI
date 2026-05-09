using CarRentalAPI.Configuration;
using CarRentalAPI.DTOs;
using CarRentalAPI.Exceptions;
using CarRentalAPI.Models;
using CarRentalAPI.Storage;
using Microsoft.Extensions.Options;

namespace CarRentalAPI.Services;

/// <summary>
/// Serwis wypożyczeń.
/// Egzekwuje obie reguły biznesowe z konfiguracji:
///   RB-1: klient max MaxActiveRentalsPerClient aktywnych wypożyczeń.
///   RB-2: klient musi mieć min. MinDriverAgeYears lat (sprawdzane przy rejestracji).
/// Dodatkowe reguły:
///   RB-3: samochód musi być dostępny.
///   RB-4: długość wypożyczenia ≤ MaxRentalDays.
/// </summary>
public class RentalService : IRentalService
{
    private readonly InMemoryStore  _store;
    private readonly RentalSettings _settings;

    public RentalService(InMemoryStore store, IOptions<RentalSettings> options)
    {
        _store    = store;
        _settings = options.Value;
    }

    public IEnumerable<RentalDto> GetAll()
        => _store.Rentals.Select(MapToDto);

    public RentalDto GetById(int id)
    {
        var rental = _store.Rentals.FirstOrDefault(r => r.Id == id)
            ?? throw new NotFoundException("Wypożyczenie", id);
        return MapToDto(rental);
    }

    public RentalDto Create(CreateRentalDto dto)
    {
        var car = _store.ActiveCars.FirstOrDefault(c => c.Id == dto.CarId)
            ?? throw new NotFoundException("Samochód", dto.CarId);

        var client = _store.ActiveClients.FirstOrDefault(c => c.Id == dto.ClientId)
            ?? throw new NotFoundException("Klient", dto.ClientId);

        // Walidacja dat
        if (dto.PlannedEnd <= dto.StartDate)
            throw new ValidationException(
                "Data zakończenia musi być późniejsza niż data rozpoczęcia.");

        if (dto.StartDate < DateTime.Today)
            throw new ValidationException(
                "Data rozpoczęcia nie może być w przeszłości.");

        var days = (dto.PlannedEnd - dto.StartDate).Days;

        // RB-4: max długość wypożyczenia (z konfiguracji)
        if (days > _settings.MaxRentalDays)
            throw new DomainException(
                $"Maksymalny dozwolony okres wypożyczenia to {_settings.MaxRentalDays} dni. " +
                $"Żądany okres: {days} dni.");

        // RB-1: limit aktywnych wypożyczeń na klienta (z konfiguracji)
        if (client.ActiveRentals >= _settings.MaxActiveRentalsPerClient)
            throw new DomainException(
                $"Klient '{client.FirstName} {client.LastName}' osiągnął limit " +
                $"{_settings.MaxActiveRentalsPerClient} aktywnych wypożyczeń jednocześnie.");

        // RB-3: samochód musi być dostępny
        if (!car.IsAvailable)
            throw new DomainException(
                $"Samochód {car.Brand} {car.Model} ({car.LicensePlate}) " +
                "jest aktualnie niedostępny.");

        var rental = new Rental
        {
            Id         = _store.NextRentalId(),
            CarId      = car.Id,
            ClientId   = client.Id,
            StartDate  = dto.StartDate,
            PlannedEnd = dto.PlannedEnd,
            TotalPrice = car.DailyRate * days,
            CreatedAt  = DateTime.UtcNow,
            Car        = car,
            Client     = client
        };

        car.IsAvailable = false;
        client.ActiveRentals++;

        _store.Rentals.Add(rental);
        return MapToDto(rental);
    }

    private static RentalDto MapToDto(Rental r) => new()
    {
        Id          = r.Id,
        CarId       = r.CarId,
        CarInfo     = r.Car is not null
                        ? $"{r.Car.Brand} {r.Car.Model} ({r.Car.LicensePlate})"
                        : $"Car #{r.CarId}",
        ClientId    = r.ClientId,
        ClientName  = r.Client is not null
                        ? $"{r.Client.FirstName} {r.Client.LastName}"
                        : $"Client #{r.ClientId}",
        StartDate   = r.StartDate,
        PlannedEnd  = r.PlannedEnd,
        ActualEnd   = r.ActualEnd,
        PlannedDays = r.PlannedDays,
        TotalPrice  = r.TotalPrice,
        LateFee     = r.LateFee,
        IsActive    = r.IsActive,
        IsOverdue   = r.IsOverdue,
        CreatedAt   = r.CreatedAt
    };
}
