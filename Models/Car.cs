namespace CarRentalAPI.Models;

/// <summary>
/// Encja główna – Samochód (Vehicle).
/// Zawiera flagę soft-delete: IsDeleted / DeletedAt.
/// Rekordy z IsDeleted=true są ukrywane w odpowiedziach API,
/// ale fizycznie pozostają w pamięci (audyt, historia).
/// </summary>
public class Car
{
    public int      Id               { get; set; }
    public string   Brand            { get; set; } = string.Empty;  // np. Toyota
    public string   Model            { get; set; } = string.Empty;  // np. Corolla
    public int      Year             { get; set; }
    public string   LicensePlate     { get; set; } = string.Empty;  // nr rejestracyjny
    public string   Color            { get; set; } = string.Empty;
    public string   FuelType         { get; set; } = string.Empty;  // Benzyna/Diesel/EV
    public decimal  DailyRate        { get; set; }                  // cena za dobę (PLN)
    public bool     IsAvailable      { get; set; } = true;          // false gdy wypożyczony
    public DateTime CreatedAt        { get; set; } = DateTime.UtcNow;

    // ── Soft Delete ───────────────────────────────────────────────────────────
    /// <summary>
    /// Jeśli true – rekord jest logicznie usunięty.
    /// Nie pojawia się w wynikach GET, ale pozostaje w magazynie danych.
    /// </summary>
    public bool      IsDeleted  { get; set; } = false;
    public DateTime? DeletedAt  { get; set; }
    public string?   DeletedBy  { get; set; }   // kto usunął (np. "admin")
}
