using System.ComponentModel.DataAnnotations;

namespace CarRentalAPI.DTOs;

// ─── Update DTOs (Etap II – PUT) ─────────────────────────────────────────────

public class UpdateCarDto
{
    [Required(ErrorMessage = "Marka jest wymagana.")]
    [StringLength(60)]
    public string Brand { get; set; } = string.Empty;

    [Required(ErrorMessage = "Model jest wymagany.")]
    [StringLength(80)]
    public string Model { get; set; } = string.Empty;

    [Range(1900, 2100)]
    public int Year { get; set; }

    [Required]
    [StringLength(30)]
    public string Color { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string FuelType { get; set; } = string.Empty;

    [Range(1, 10000, ErrorMessage = "Stawka dzienna musi być > 0.")]
    public decimal DailyRate { get; set; }
}

public class UpdateClientDto
{
    [Required(ErrorMessage = "Imię jest wymagane.")]
    [StringLength(80)]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nazwisko jest wymagane.")]
    [StringLength(80)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress(ErrorMessage = "Nieprawidłowy format e-mail.")]
    public string Email { get; set; } = string.Empty;

    [Phone]
    public string PhoneNumber { get; set; } = string.Empty;
}
