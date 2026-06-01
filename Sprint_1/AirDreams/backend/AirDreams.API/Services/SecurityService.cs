using AirDreams.API.Repositories;
using AirDreams.API.Services.Interfaces;

namespace AirDreams.API.Services
{
    public class SecurityService : ISecurityService
    {
        private readonly ISecurityRepository _securityRepository;

        public SecurityService(ISecurityRepository securityRepository)
        {
            _securityRepository = securityRepository;
        }

        public async Task ValidateApiKeyAsync(string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new UnauthorizedAccessException("INVALID_API_KEY: La API key es requerida.");
            }

            var airline = await _securityRepository.ValidateApiKeyAsync(apiKey);

            if (string.IsNullOrEmpty(airline))
            {
                throw new UnauthorizedAccessException("INVALID_API_KEY: La API key proporcionada no es válida.");
            }
        }
    }
}
