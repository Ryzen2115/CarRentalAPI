using CarRentalAPI.DTOs;

namespace CarRentalAPI.Services;

public interface IRentalService
{
    IEnumerable<RentalDto> GetAll();
    RentalDto?             GetById(int id);
    RentalDto              Create(CreateRentalDto dto);
}
