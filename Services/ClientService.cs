using CarRentalAPI.Configuration;
using CarRentalAPI.DTOs;
using CarRentalAPI.Exceptions;
using CarRentalAPI.Models;
using CarRentalAPI.Storage;
using Microsoft.Extensions.Options;

namespace CarRentalAPI.Services;

public class ClientService : IClientService
{
    private readonly InMemoryStore  _store;
    private readonly RentalSettings _settings;

    public ClientService(InMemoryStore store, IOptions<RentalSettings> options)
    {
        _store    = store;
        _settings = options.Value;
    }

    public IEnumerable<ClientDto> GetAll()
        => _store.ActiveClients.Select(MapToDto);

    public ClientDto GetById(int id)
    {
        var client = _store.ActiveClients.FirstOrDefault(c => c.Id == id)
            ?? throw new NotFoundException("Klient", id);
        return MapToDto(client);
    }

    public ClientDto Create(CreateClientDto dto)
    {
        if (_store.ActiveClients.Any(c =>
                c.Email.Equals(dto.Email.Trim(), StringComparison.OrdinalIgnoreCase)))
            throw new DomainException($"Klient z e-mailem '{dto.Email}' już istnieje.");

        // Reguła biznesowa #2 (z konfiguracji): minimalny wiek kierowcy
        var age = CalculateAge(dto.DateOfBirth);
        if (age < _settings.MinDriverAgeYears)
            throw new DomainException(
                $"Klient musi mieć co najmniej {_settings.MinDriverAgeYears} lat. " +
                $"Podany wiek: {age} lat.");

        var client = new Client
        {
            Id              = _store.NextClientId(),
            FirstName       = dto.FirstName.Trim(),
            LastName        = dto.LastName.Trim(),
            Email           = dto.Email.Trim().ToLower(),
            PhoneNumber     = dto.PhoneNumber.Trim(),
            DriverLicenseNo = dto.DriverLicenseNo.Trim().ToUpper(),
            DateOfBirth     = dto.DateOfBirth,
            RegisteredAt    = DateTime.UtcNow
        };

        _store.Clients.Add(client);
        return MapToDto(client);
    }

    public ClientDto Update(int id, UpdateClientDto dto)
    {
        var client = _store.ActiveClients.FirstOrDefault(c => c.Id == id)
            ?? throw new NotFoundException("Klient", id);

        // Sprawdź czy nowy e-mail nie koliduje z innym klientem
        if (_store.ActiveClients.Any(c =>
                c.Id != id &&
                c.Email.Equals(dto.Email.Trim(), StringComparison.OrdinalIgnoreCase)))
            throw new DomainException($"Inny klient z e-mailem '{dto.Email}' już istnieje.");

        client.FirstName   = dto.FirstName.Trim();
        client.LastName    = dto.LastName.Trim();
        client.Email       = dto.Email.Trim().ToLower();
        client.PhoneNumber = dto.PhoneNumber.Trim();

        return MapToDto(client);
    }

    public void SoftDelete(int id, SoftDeleteDto dto)
    {
        var client = _store.Clients.FirstOrDefault(c => c.Id == id)
            ?? throw new NotFoundException("Klient", id);

        if (client.IsDeleted)
            throw new NotFoundException("Klient", id);

        if (client.ActiveRentals > 0)
            throw new DomainException(
                $"Nie można usunąć klienta '{client.FirstName} {client.LastName}', " +
                $"który ma {client.ActiveRentals} aktywnych wypożyczeń.");

        client.IsDeleted = true;
        client.DeletedAt = DateTime.UtcNow;
        client.DeletedBy = dto.DeletedBy;
    }

    private ClientDto MapToDto(Client c) => new()
    {
        Id              = c.Id,
        FirstName       = c.FirstName,
        LastName        = c.LastName,
        Email           = c.Email,
        PhoneNumber     = c.PhoneNumber,
        DriverLicenseNo = c.DriverLicenseNo,
        DateOfBirth     = c.DateOfBirth,
        ActiveRentals   = c.ActiveRentals,
        CanRent         = c.ActiveRentals < _settings.MaxActiveRentalsPerClient,
        RegisteredAt    = c.RegisteredAt
    };

    private static int CalculateAge(DateTime dob)
    {
        var today = DateTime.Today;
        var age   = today.Year - dob.Year;
        if (dob.Date > today.AddYears(-age)) age--;
        return age;
    }
}
