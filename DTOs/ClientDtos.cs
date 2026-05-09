using System.ComponentModel.DataAnnotations;

namespace CarRentalAPI.DTOs;

// ─── Client DTOs ──────────────────────────────────────────────────────────────

public class CreateClientDto
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

    [Required(ErrorMessage = "Numer prawa jazdy jest wymagany.")]
    [StringLength(20, MinimumLength = 5)]
    public string DriverLicenseNo { get; set; } = string.Empty;

    [Required(ErrorMessage = "Data urodzenia jest wymagana.")]
    public DateTime DateOfBirth { get; set; }
}

public class ClientDto
{
    public int      Id              { get; set; }
    public string   FirstName       { get; set; } = string.Empty;
    public string   LastName        { get; set; } = string.Empty;
    public string   FullName        => $"{FirstName} {LastName}";
    public string   Email           { get; set; } = string.Empty;
    public string   PhoneNumber     { get; set; } = string.Empty;
    public string   DriverLicenseNo { get; set; } = string.Empty;
    public DateTime DateOfBirth     { get; set; }
    public int      ActiveRentals   { get; set; }
    public bool     CanRent         { get; set; }
    public DateTime RegisteredAt    { get; set; }
}
