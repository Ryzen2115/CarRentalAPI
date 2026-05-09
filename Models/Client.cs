namespace CarRentalAPI.Models;

/// <summary>
/// Użytkownik systemu – Klient (Client).
/// Soft-delete: klient usunięty logicznie nie może tworzyć nowych wypożyczeń,
/// ale jego historia wypożyczeń jest zachowana.
/// </summary>
public class Client
{
    public int      Id               { get; set; }
    public string   FirstName        { get; set; } = string.Empty;
    public string   LastName         { get; set; } = string.Empty;
    public string   Email            { get; set; } = string.Empty;
    public string   PhoneNumber      { get; set; } = string.Empty;
    public string   DriverLicenseNo  { get; set; } = string.Empty;

    /// <summary>
    /// Reguła biznesowa #2: klient musi mieć co najmniej MinDriverAgeYears lat.
    /// Weryfikowana względem tej daty urodzenia.
    /// </summary>
    public DateTime DateOfBirth      { get; set; }

    public int      ActiveRentals    { get; set; } = 0;
    public DateTime RegisteredAt     { get; set; } = DateTime.UtcNow;

    // ── Soft Delete ───────────────────────────────────────────────────────────
    public bool      IsDeleted  { get; set; } = false;
    public DateTime? DeletedAt  { get; set; }
    public string?   DeletedBy  { get; set; }
}
