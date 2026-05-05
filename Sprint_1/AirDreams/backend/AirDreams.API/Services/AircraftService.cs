using AirDreams.API.Models;
using AirDreams.API.Repositories;

namespace AirDreams.API.Services
{
    public class AircraftService
    {
        private readonly AircraftRepository aircraftRepository;

        public AircraftService(AircraftRepository aircraftRepository)
        {
            this.aircraftRepository = aircraftRepository;
        }

        public List<AircraftModel> GetAircrafts()
        {
            return aircraftRepository.GetAircrafts();
        }

    
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

        public string DeleteAircraft(string plateNumber)
        {
            var result = string.Empty;

            try
            {
                var isDeleted = aircraftRepository.DeleteAircraft(plateNumber);

                if (!isDeleted)
                {
                    result = "No se pudo eliminar la aeronave";
                }
 
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            return result;
        }

        public AircraftModel? GetAircraftByPlateNumber(string plateNumber)
        {
            return aircraftRepository.GetAircraftByPlateNumber(plateNumber);
        }

        public string UpdateAircraft(AircraftModel aircraft)
        {
            var result = string.Empty;

            try
            {
                var isUpdated = aircraftRepository.UpdateAircraft(aircraft);

                if (!isUpdated)
                {
                    result = "No se pudo actualizar la aeronave.";
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            return result;
        }
    }
}