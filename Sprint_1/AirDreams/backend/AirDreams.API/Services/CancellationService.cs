using AirDreams.API.Repositories.Interfaces;
using AirDreams.API.Services.Interfaces;

namespace AirDreams.API.Services
{
    public class CancellationService : ICancellationService
    {
        private readonly ICancellationRepository _repository;
        private readonly IEmailService _emailService;

        public CancellationService(
            ICancellationRepository repository,
            IEmailService emailService)
        {
            _repository = repository;
            _emailService = emailService;
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

        public async Task RequestCancellationAsync(string transactionId)
        {
            if (string.IsNullOrWhiteSpace(transactionId))
                throw new ArgumentException("El código de reserva es requerido.");

            var exists = await _repository.ItineraryExistsAsync(transactionId);

            if (!exists)
                throw new KeyNotFoundException("La reserva no existe.");

            var email =
                await _repository.GetBuyerEmailByTransactionIdAsync(transactionId);

            if (string.IsNullOrWhiteSpace(email))
                throw new InvalidOperationException(
                    "No se encontró un correo asociado a la reserva.");

            var token = Guid.NewGuid().ToString();

            var expirationDate = DateTime.UtcNow.AddHours(24);

            await _repository.SaveCancellationTokenAsync(
                transactionId,
                token,
                expirationDate);

            await _emailService.SendCancellationEmailAsync(
                email,
                token);
        }

        public async Task ConfirmCancellationAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("El token de cancelación es requerido.");

            var cancellationToken =
                await _repository.GetCancellationTokenAsync(token);

            if (cancellationToken == null)
                throw new KeyNotFoundException("El token de cancelación no existe.");

            if (cancellationToken.Value.isUsed)
                throw new InvalidOperationException(
                    "El token de cancelación ya ha sido utilizado.");

            if (cancellationToken.Value.expirationDate < DateTime.UtcNow)
                throw new InvalidOperationException(
                    "El token de cancelación ha expirado.");

            await CancelReservationAsync(cancellationToken.Value.transactionId);

            await _repository.MarkCancellationTokenAsUsedAsync(token);
        }
    }
}