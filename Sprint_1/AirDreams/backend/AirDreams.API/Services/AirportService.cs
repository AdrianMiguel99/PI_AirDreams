using AirDreams.API.Models.Dtos;
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

        public async Task<List<AirportDto>> GetAllAsync()
        {
            var airports = await _repository.GetAllAsync();
            return airports.Select(a => MapToDto(a)).ToList();
        }

        public async Task<AirportDto?> GetByCodeAsync(string code)
        {
            var airport = await _repository.GetByCodeAsync(code);
            return airport is null ? null : MapToDto(airport);
        }

        public async Task<AirportDto> CreateAsync(CreateAirportDto dto, byte adminId)
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

        private static AirportDto MapToDto(Airport a) => new()
        {
            Code = a.CodeAirport,
            Name = a.NameAirport,
            City = a.City,
            Country = a.Country,
            TimeZone = a.TimeZone
        };
    }
}