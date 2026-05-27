using AirDreams.API.DTOs;
using AirDreams.API.Models;

namespace AirDreams.API.Services
{
    public interface IRouteService
    {
        Task<int> CreateRouteWithFrequenciesAsync(CreateRouteModel model, byte adminID);
        Task<IEnumerable<RouteDTO>> GetAllAsync();
    }
}
