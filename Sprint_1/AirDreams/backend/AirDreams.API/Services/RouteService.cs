using AirDreams.API.DTOs;
using AirDreams.API.Models;
using AirDreams.API.Repositories;

namespace AirDreams.API.Services
{
    public class RouteService : IRouteService
    {
        private readonly IRouteRepository _routeRepository;

        public RouteService(IRouteRepository routeRepository)
        {
            _routeRepository = routeRepository;
        }

        public async Task<int> CreateRouteWithFrequenciesAsync(CreateRouteModel model, byte adminId)
        {
            ValidateRoute(model);

            model.AdminID = adminId;

            model.CodeAirportSalida = model.CodeAirportSalida.Trim().ToUpper();
            model.CodeAirportLlegada = model.CodeAirportLlegada.Trim().ToUpper();
            model.Modelo = model.Modelo.Trim().ToUpper();

            return await _routeRepository.CreateRouteWithFrequenciesAsync(model);
        }

        public async Task<IEnumerable<RouteDTO>> GetAllAsync()
        {
            return await _routeRepository.GetAllAsync();
        }

        public async Task<RouteDeleteResultDTO> DeleteAsync(int routeId)
        {
            if (routeId <= 0)
                throw new ArgumentException("El ID de la ruta no es válido.");

            return await _routeRepository.DeleteAsync(routeId);
        }

        private void ValidateRoute(CreateRouteModel model)
        {
            if (model == null)
                throw new ArgumentException("Los datos de la ruta son requeridos.");

            if (string.IsNullOrWhiteSpace(model.CodeAirportSalida))
                throw new ArgumentException("El aeropuerto de salida es requerido.");

            if (string.IsNullOrWhiteSpace(model.CodeAirportLlegada))
                throw new ArgumentException("El aeropuerto de llegada es requerido.");

            if (model.CodeAirportSalida.Equals(model.CodeAirportLlegada, StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("El aeropuerto de origen y destino no pueden ser el mismo.");

            if (string.IsNullOrWhiteSpace(model.Modelo))
                throw new ArgumentException("La aeronave es requerida.");

            if (model.FirstClassPrice < 0 || model.TuristClassPrice < 0)
                throw new ArgumentException("Los precios no pueden ser negativos.");

            if (model.LuggagePrice < 0 || model.carryOnPrice < 0)
                throw new ArgumentException("El precio de equipaje no puede ser negativo.");

            if (model.carryOnMaxWeight < 0 || model.LuggageMaxWeight < 0)
                throw new ArgumentException("El peso del equipaje no puede ser negativo");

            if(model.porcentageMultiplier < 0 || model.porcentageMultiplier > 100)
                throw new ArgumentException("El multiplicador no puede ser menor a 0 ni mayor a 100");

            if (model.Distance <= 0)
                throw new ArgumentException("La distancia debe ser mayor a cero.");

            if (model.Frequencies == null || !model.Frequencies.Any())
                throw new ArgumentException("Debe registrar al menos una frecuencia.");

            foreach (var frequency in model.Frequencies)
            {
                if (string.IsNullOrWhiteSpace(frequency.DayOfWeek))
                    throw new ArgumentException("Cada frecuencia debe tener un día.");

            
            }
        }
    }
}
