using System.ComponentModel.DataAnnotations;

namespace CarRentalAPI.DTOs;

// ─── Car DTOs ─────────────────────────────────────────────────────────────────

public class CreateCarDto
{
    [Required(ErrorMessage = "Marka jest wymagana.")]
    [StringLength(60)]
    public string Brand { get; set; } = string.Empty;

    [Required(ErrorMessage = "Model jest wymagany.")]
    [StringLength(80)]
    public string Model { get; set; } = string.Empty;

    [Range(1900, 2100, ErrorMessage = "Rok produkcji musi być między 1900 a 2100.")]
    public int Year { get; set; }

    [Required(ErrorMessage = "Numer rejestracyjny jest wymagany.")]
    [StringLength(10, MinimumLength = 4)]
    public string LicensePlate { get; set; } = string.Empty;

    [Required]
    [StringLength(30)]
    public string Color { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string FuelType { get; set; } = string.Empty;

    [Range(1, 10000, ErrorMessage = "Stawka dzienna musi być > 0.")]
    public decimal DailyRate { get; set; }
}

public class CarDto
{
    public int      Id           { get; set; }
    public string   Brand        { get; set; } = string.Empty;
    public string   Model        { get; set; } = string.Empty;
    public int      Year         { get; set; }
    public string   LicensePlate { get; set; } = string.Empty;
    public string   Color        { get; set; } = string.Empty;
    public string   FuelType     { get; set; } = string.Empty;
    public decimal  DailyRate    { get; set; }
    public bool     IsAvailable  { get; set; }
    public DateTime CreatedAt    { get; set; }
}
