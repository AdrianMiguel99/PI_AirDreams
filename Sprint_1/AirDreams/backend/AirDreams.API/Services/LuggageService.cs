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
    }
}
