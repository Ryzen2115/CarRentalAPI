using System.ComponentModel.DataAnnotations;

namespace CarRentalAPI.DTOs;

// ─── Rental DTOs ──────────────────────────────────────────────────────────────

public class CreateRentalDto
{
    [Required]
    [Range(1, int.MaxValue)]
    public int CarId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int ClientId { get; set; }

    [Required(ErrorMessage = "Data rozpoczęcia jest wymagana.")]
    public DateTime StartDate { get; set; }

    [Required(ErrorMessage = "Planowana data zakończenia jest wymagana.")]
    public DateTime PlannedEnd { get; set; }
}

public class RentalDto
{
    public int       Id          { get; set; }
    public int       CarId       { get; set; }
    public string    CarInfo     { get; set; } = string.Empty;   // "Toyota Corolla (WA 12345)"
    public int       ClientId    { get; set; }
    public string    ClientName  { get; set; } = string.Empty;
    public DateTime  StartDate   { get; set; }
    public DateTime  PlannedEnd  { get; set; }
    public DateTime? ActualEnd   { get; set; }
    public int       PlannedDays { get; set; }
    public decimal   TotalPrice  { get; set; }
    public decimal   LateFee     { get; set; }
    public bool      IsActive    { get; set; }
    public bool      IsOverdue   { get; set; }
    public DateTime  CreatedAt   { get; set; }
}
