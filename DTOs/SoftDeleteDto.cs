using System.ComponentModel.DataAnnotations;

namespace CarRentalAPI.DTOs;

/// <summary>
/// DTO wejściowy dla operacji soft-delete (DELETE logiczny).
/// Pozwala opcjonalnie wskazać, kto usuwa rekord.
/// </summary>
public class SoftDeleteDto
{
    [StringLength(100)]
    public string DeletedBy { get; set; } = "system";
}
