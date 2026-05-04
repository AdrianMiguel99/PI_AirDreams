using AirDreams.API.Models;
using AirDreams.API.DTOs;

public  interface IRouteRepository
{
    Task<int> CreateRouteWithFrequenciesAsync(CreateRouteModel model);
    Task<IEnumerable<RouteDTO>> GetAllAsync(); 
}