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
            if (await _repository.ExistsAsync(dto.IdPassenger))
                throw new InvalidOperationException("El pasajero ya existe.");

            var passenger = new Passenger
            {
                IdPassenger = dto.IdPassenger,
                NamePassenger = dto.NamePassenger.Trim(),
                LastnamesPassenger = dto.LastnamesPassenger.Trim(),
                EmailPassenger = dto.EmailPassenger.Trim().ToLower(),
                Telephone = dto.Telephone,
                Country = dto.Country.Trim()
            };

            var created = await _repository.CreateAsync(passenger);
            return MapToDto(created);
        }

        public async Task<PassengerDto?> GetByIdAsync(int idPassenger)
        {
            var passenger = await _repository.GetByIdAsync(idPassenger);
            return passenger is null ? null : MapToDto(passenger);
        }

        public async Task<PassengerDto?> GetPassengerByNameAsync(string fullName)
        {
            var names = fullName.Trim().Split(' ', 2);

            if (names.Length < 2)
                return null;

            var name = names[0];
            var lastNames = names[1];

            var passenger = await _repository.GetPassengerByNameAsync(name);
            return passenger is null ? null : MapToDto(passenger);
        }

        private static PassengerDto MapToDto(Passenger passenger) => new()
        {
            IdPassenger = passenger.IdPassenger,
            NamePassenger = passenger.NamePassenger,
            LastnamesPassenger = passenger.LastnamesPassenger,
            EmailPassenger = passenger.EmailPassenger,
            Telephone = passenger.Telephone,
            Country = passenger.Country
        };
    }
}
