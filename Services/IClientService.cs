using CarRentalAPI.DTOs;

namespace CarRentalAPI.Services;

public interface IClientService
{
    IEnumerable<ClientDto> GetAll();
    ClientDto              GetById(int id);
    ClientDto              Create(CreateClientDto dto);
    ClientDto              Update(int id, UpdateClientDto dto);
    void                   SoftDelete(int id, SoftDeleteDto dto);
}
