using CarRentalAPI.DTOs;

namespace CarRentalAPI.Services;

public interface ICarService
{
    IEnumerable<CarDto> GetAll();
    CarDto              GetById(int id);
    CarDto              Create(CreateCarDto dto);
    CarDto              Update(int id, UpdateCarDto dto);
    void                SoftDelete(int id, SoftDeleteDto dto);
}
