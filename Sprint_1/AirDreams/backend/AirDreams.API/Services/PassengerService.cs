using AirDreams.API.DTOs;
using AirDreams.API.Models.Entities;
using AirDreams.API.Repositories;

namespace AirDreams.API.Services
{
    public class PassengerService : IPassengerService
    {
        private readonly IPassengerRepository _repository;

        public PassengerService(IPassengerRepository repository)
        {
            _repository = repository;
        }

        public async Task<PassengerDto> CreateAsync(CreatePassengerDto dto)
        {
            var passport = dto.Passport.Trim().ToUpper();

            if (await _repository.ExistsAsync(dto.IdPassenger, passport))
                throw new InvalidOperationException("El pasajero ya existe.");

            var passenger = new Passenger
            {
                IdPassenger = dto.IdPassenger,
                Passport = passport,
                NamePassenger = dto.NamePassenger.Trim(),
                LastnamesPassenger = dto.LastnamesPassenger.Trim(),
                EmailPassenger = dto.EmailPassenger.Trim().ToLower(),
                Telephone = dto.Telephone,
                CountryCode = dto.CountryCode
            };

            var created = await _repository.CreateAsync(passenger);
            return MapToDto(created);
        }

        public async Task<PassengerDto?> GetByIdAsync(int idPassenger, string passport)
        {
            var passenger = await _repository.GetByIdAsync(idPassenger, passport.Trim().ToUpper());
            return passenger is null ? null : MapToDto(passenger);
        }

        private static PassengerDto MapToDto(Passenger passenger) => new()
        {
            IdPassenger = passenger.IdPassenger,
            Passport = passenger.Passport,
            NamePassenger = passenger.NamePassenger,
            LastnamesPassenger = passenger.LastnamesPassenger,
            EmailPassenger = passenger.EmailPassenger,
            Telephone = passenger.Telephone,
            CountryCode = passenger.CountryCode
        };
    }
}
