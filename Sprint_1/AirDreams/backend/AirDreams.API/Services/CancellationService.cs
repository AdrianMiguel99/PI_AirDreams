using AirDreams.API.Repositories.Interfaces;
using AirDreams.API.Services.Interfaces;

namespace AirDreams.API.Services
{
    public class CancellationService : ICancellationService
    {
        private readonly ICancellationRepository _repository;

        public CancellationService(ICancellationRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> CancelReservationAsync(string transactionId)
        {
            if (string.IsNullOrWhiteSpace(transactionId))
                throw new ArgumentException("El código de reserva es requerido.");

            var exists = await _repository.ItineraryExistsAsync(transactionId);

            if (!exists)
                throw new KeyNotFoundException("La reserva no existe.");

            var isCancelled =
                await _repository.IsItineraryCancelledAsync(transactionId);

            if (isCancelled)
                throw new InvalidOperationException("La reserva ya se encuentra cancelada.");

            await _repository.CancelItineraryAsync(transactionId);

            return true;
        }
    }
}