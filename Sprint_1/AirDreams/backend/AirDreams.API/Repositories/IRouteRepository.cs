using AirDreams.API.Models;

public interface IRouteRepository
{
    Task<int> CreateRouteWithFrequenciesAsync(CreateRouteModel model);
    Task<IEnumerable<CreateRouteModel>> GetAllAsync(); 
}