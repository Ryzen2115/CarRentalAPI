namespace CarRentalAPI.Models;

/// <summary>
/// Relacja logiczna: wypożyczenie samochodu przez klienta.
/// Wypożyczenia NIE są soft-deletowane – stanowią rejestr finansowy.
/// </summary>
public class Rental
{
    public int       Id          { get; set; }
    public int       CarId       { get; set; }
    public int       ClientId    { get; set; }
    public DateTime  StartDate   { get; set; }
    public DateTime  PlannedEnd  { get; set; }
    public DateTime? ActualEnd   { get; set; }
    public decimal   TotalPrice  { get; set; }
    public decimal   LateFee     { get; set; } = 0;
    public DateTime  CreatedAt   { get; set; } = DateTime.UtcNow;

    public bool IsActive   => ActualEnd is null;
    public bool IsOverdue  => IsActive && DateTime.UtcNow > PlannedEnd;
    public int  PlannedDays => (PlannedEnd - StartDate).Days;

    // Nawigacja (in-memory, bez EF)
    public Car?    Car    { get; set; }
    public Client? Client { get; set; }
}
