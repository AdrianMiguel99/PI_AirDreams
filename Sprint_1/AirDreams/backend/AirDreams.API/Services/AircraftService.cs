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
        public string AddAircraft(AircraftModel aircraft)
        {

            var result = string.Empty;
            try
            {
                var isAdded = aircraftRepository.AddAircraft(aircraft);
                if (!isAdded)
                {
                    result = "Error al registrar la aeronave.";
                }
                
            }catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }
    }
}