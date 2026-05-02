using AirDreams.API.Models;
using AirDreams.API.Repository;

namespace AirDreams.API.Services
{
    public class AircraftService
    {
        private readonly AircraftRepository aircraftRepository;

        public AircraftService(AircraftRepository aircraftRepository)
        {
            this.aircraftRepository = aircraftRepository;
        }

        // 🔹 Obtener aeronaves
        public List<AircraftModel> GetAircrafts()
        {
            return aircraftRepository.GetAircrafts();
        }

        // 🔹 Registrar aeronave
        public void AddAircraft(AircraftModel aircraft)
        {
            // aquí podrías meter validaciones después
            aircraftRepository.AddAircraft(aircraft);
        }
    }
}