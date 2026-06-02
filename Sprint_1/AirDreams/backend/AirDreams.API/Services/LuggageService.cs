using AirDreams.API.Models;
using AirDreams.API.Repositories;
using AirDreams.API.Services.Interfaces;

namespace AirDreams.API.Services
{
    public class LuggageService : ILuggageService
    {
        private readonly ILuggageRepository _luggageRepository;
        private readonly IPassengerService _passengerService;

        public LuggageService(ILuggageRepository luggageRepository, IPassengerService passengerService)
        {
            _luggageRepository = luggageRepository;
            _passengerService = passengerService;
        }

        public async Task<(bool success, string message)> RegisterLuggageAsync(LuggageRegistrationModel model)
        {
            try
            {
                var passenger = await _passengerService.GetPassengerByNameAsync(model.FullName);
                
                if (passenger == null)
                {
                    return (false, "Pasajero no encontrado");
                }

                int registeredCount = 0;

                foreach (var item in model.LuggageItems)
                {
                    var luggageNumber = await _luggageRepository.CreateLuggageAsync(
                        item.Type,
                        item.Quantity
                    );

                    if (string.IsNullOrEmpty(luggageNumber))
                    {
                        continue;
                    }

                    var luggageRegistered = await _luggageRepository.RegisterLuggageAsync(
                        passenger.IdPassenger,
                        model.TransactionIdItinerary,
                        luggageNumber
                    );

                    if (luggageRegistered)
                    {
                        registeredCount++;
                    }
                }

                if (registeredCount > 0)
                {
                    return (true, $"Se registraron {registeredCount} equipajes exitosamente");
                }

                return (false, "No se pudo registrar ningún equipaje");
            }
            catch (Exception ex)
            {
                return (false, $"Error durante el registro de equipaje: {ex.Message}");
            }
        }

        public async Task<(bool success, string message)> CheckAvailabilityAsync(string flightId, int routeId, decimal luggageWeight, decimal carryOnWeight)
        {
            try
            {
                var luggageCheck = await _luggageRepository.CheckLuggageWeightAsync(flightId, routeId, luggageWeight);
                var carryOnCheck = await _luggageRepository.CheckCarryOnWeightAsync(flightId, routeId, carryOnWeight);

                if (!luggageCheck && !carryOnCheck)
                {
                    return (false, "No hay espacio suficiente para el equipaje y el equipaje de mano");
                }

                if (!luggageCheck)
                {
                    return (false, "No hay espacio suficiente para el equipaje");
                }

                if (!carryOnCheck)
                {
                    return (false, "No hay espacio suficiente para el equipaje de mano");
                }

                return (true, "Hay espacio suficiente para el equipaje y el equipaje de mano");
            }
            catch (Exception ex)
            {
                return (false, $"Error al validar la disponibilidad: {ex.Message}");
            }
        }
    }
}
