namespace CarRentalAPI.Configuration;

/// <summary>
/// Silnie typowana konfiguracja biznesowa czytana z appsettings.json → "RentalSettings".
/// Wstrzykiwana przez IOptions&lt;RentalSettings&gt; wszędzie tam, gdzie potrzebna.
/// </summary>
public class RentalSettings
{
    public const string SectionName = "RentalSettings";

    /// <summary>Maksymalna długość wypożyczenia w dniach. (domyślnie 30)</summary>
    public int MaxRentalDays { get; set; } = 30;

    /// <summary>
    /// Reguła biznesowa #1:
    /// Jeden klient nie może mieć więcej niż N aktywnych wypożyczeń jednocześnie.
    /// </summary>
    public int MaxActiveRentalsPerClient { get; set; } = 3;

    /// <summary>Procentowa opłata karna za każdy dzień spóźnienia (np. 10 = 10%).</summary>
    public double DailyLateFeePercent { get; set; } = 10;

    /// <summary>
    /// Reguła biznesowa #2:
    /// Minimalny wiek kierowcy wymagany do wypożyczenia pojazdu.
    /// </summary>
    public int MinDriverAgeYears { get; set; } = 21;
}
