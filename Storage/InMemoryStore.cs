using CarRentalAPI.Models;

namespace CarRentalAPI.Storage;

/// <summary>
/// Singleton zastępujący bazę danych w Etapie I.
/// Przechowuje dane w pamięci RAM.
/// Soft-delete: rekordy z IsDeleted=true pozostają w listach,
/// ale serwisy filtrują je przed zwróceniem do klienta.
/// </summary>
public class InMemoryStore
{
    private int _carIdCounter    = 0;
    private int _clientIdCounter = 0;
    private int _rentalIdCounter = 0;

    public int NextCarId()    => Interlocked.Increment(ref _carIdCounter);
    public int NextClientId() => Interlocked.Increment(ref _clientIdCounter);
    public int NextRentalId() => Interlocked.Increment(ref _rentalIdCounter);

    // Pełne kolekcje – zawierają też rekordy soft-deleted
    public List<Car>    Cars    { get; } = new();
    public List<Client> Clients { get; } = new();
    public List<Rental> Rentals { get; } = new();

    // Wygodne widoki – tylko aktywne (nie-usunięte) rekordy
    public IEnumerable<Car>    ActiveCars    => Cars.Where(c => !c.IsDeleted);
    public IEnumerable<Client> ActiveClients => Clients.Where(c => !c.IsDeleted);

    public InMemoryStore()
    {
        SeedCars();
        SeedClients();
    }

    private void SeedCars()
    {
        var seed = new[]
        {
            new Car { Id = NextCarId(), Brand = "Toyota",     Model = "Corolla",   Year = 2022,
                      LicensePlate = "WA 12345", Color = "Biały",   FuelType = "Benzyna", DailyRate = 180 },
            new Car { Id = NextCarId(), Brand = "Volkswagen", Model = "Golf",      Year = 2021,
                      LicensePlate = "KR 98765", Color = "Szary",   FuelType = "Diesel",  DailyRate = 200 },
            new Car { Id = NextCarId(), Brand = "Tesla",      Model = "Model 3",   Year = 2023,
                      LicensePlate = "GD 55500", Color = "Czarny",  FuelType = "EV",      DailyRate = 350 },
            new Car { Id = NextCarId(), Brand = "Skoda",      Model = "Octavia",   Year = 2020,
                      LicensePlate = "PO 44411", Color = "Srebrny", FuelType = "Diesel",  DailyRate = 160 },
        };
        Cars.AddRange(seed);
    }

    private void SeedClients()
    {
        var seed = new[]
        {
            new Client { Id = NextClientId(), FirstName = "Jan",    LastName = "Kowalski",
                         Email = "jan.kowalski@example.com", PhoneNumber = "600-111-222",
                         DriverLicenseNo = "KOW123456",
                         DateOfBirth = new DateTime(1990, 5, 15) },
            new Client { Id = NextClientId(), FirstName = "Maria",  LastName = "Nowak",
                         Email = "maria.nowak@example.com",  PhoneNumber = "700-333-444",
                         DriverLicenseNo = "NOW987654",
                         DateOfBirth = new DateTime(1985, 11, 3) },
        };
        Clients.AddRange(seed);
    }
}
