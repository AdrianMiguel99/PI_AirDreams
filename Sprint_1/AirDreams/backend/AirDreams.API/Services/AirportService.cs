using AirDreams.API.DTOs;
using AirDreams.API.Models.Entities;
using AirDreams.API.Repositories;

namespace AirDreams.API.Services
{
    public class AirportService : IAirportService
    {
        private readonly IAirportRepository _repository;

        public AirportService(IAirportRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<AirportDTO>> GetAllAsync()
        {
            var airports = await _repository.GetAllAsync();
            return airports.Select(a => MapToDto(a)).ToList();
        }

        public async Task<AirportDTO?> GetByCodeAsync(string code)
        {
            var airport = await _repository.GetByCodeAsync(code);
            return airport is null ? null : MapToDto(airport);
        }

        public async Task<AirportDTO> CreateAsync(CreateAirportDTO dto, byte adminId)
        {
            string code = dto.Code.ToUpperInvariant(); 

            if (await _repository.ExistsAsync(code))
                throw new InvalidOperationException("El código de aeropuerto ya existe.");

            var airport = new Airport
            {
                CodeAirport = code,
                NameAirport = dto.Name,
                City = dto.City,
                Country = dto.Country,
                AdminID = adminId,
                TimeZone = dto.TimeZone
            };

            var created = await _repository.CreateAsync(airport);
            return MapToDto(created);
        }

        private static AirportDTO MapToDto(Airport a) => new()
        {
            Code = a.CodeAirport,
            Name = a.NameAirport,
            City = a.City,
            Country = a.Country,
            TimeZone = a.TimeZone,
            isActive = a.isActive
        };

        public async Task UpdateAsync(string code, UpdateAirportDTO dto)
        {
            var airport = await _repository.GetByCodeAsync(code);
            if (airport == null)
                throw new KeyNotFoundException($"No se encontró el aeropuerto con código {code}.");

            var updated = await _repository.UpdateAsync(code, dto.Name);
            if (!updated)
                throw new Exception("No se pudo actualizar el aeropuerto.");
        }
        public async Task<(bool success, string message)> DeleteAsync(string code)
        {
            code = code.ToUpperInvariant();
            var airport = await _repository.GetByCodeAsync(code);
            if (airport == null)
                return (false, "Aeropuerto no encontrado.");

            bool inUse = await _repository.IsAirportInUseAsync(code);
            return await _repository.DeleteAirportAsync(code, inUse);
        }
    }
}