using AirDreams.API.DTOs;
using AirDreams.API.Models;
using AirDreams.API.Repositories;
using AirDreams.API.Services.Interfaces;

namespace AirDreams.API.Services
{
    public class AircraftService : IAircraftService
    {
        private readonly IAircraftRepository aircraftRepository;

        public AircraftService(IAircraftRepository aircraftRepository)
        {
            this.aircraftRepository = aircraftRepository;
        }

        public List<AircraftViewModel> GetAircrafts()
        {
            return aircraftRepository.GetAircrafts();
        }

        public string AddAircraft(AircraftModel aircraft)
        {
            var validationResult = ValidateAircraft(aircraft);

            if (!string.IsNullOrEmpty(validationResult))
            {
                return validationResult;
            }

            if (aircraftRepository.ExistsByModel(aircraft.Modelo))
            {
                return "Ya existe una aeronave con ese modelo.";
            }

            try
            {
                var isAdded = aircraftRepository.AddAircraft(aircraft);

                if (!isAdded)
                {
                    return "Error al registrar la aeronave.";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return string.Empty;
        }

        public AircraftDeleteResultDTO DeleteAircraft(string modelo)
        {
            if (string.IsNullOrWhiteSpace(modelo))
            {
                throw new ArgumentException("El modelo es obligatorio.");
            }

            return aircraftRepository.DeleteAircraft(modelo.Trim());
        }

        public AircraftViewModel? GetAircraftByModel(string modelo)
        {
            return aircraftRepository.GetAircraftByModel(modelo);
        }

        public string UpdateAircraft(AircraftModel aircraft)
        {
            var validationResult = ValidateAircraft(aircraft);

            if (!string.IsNullOrEmpty(validationResult))
            {
                return validationResult;
            }

            try
            {
                var isUpdated = aircraftRepository.UpdateAircraft(aircraft);

                if (!isUpdated)
                {
                    return "No se pudo actualizar la aeronave.";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return string.Empty;
        }

        private string ValidateAircraft(AircraftModel aircraft)
        {
            if (aircraft == null)
            {
                return "Los datos de la aeronave son obligatorios.";
            }

            if (string.IsNullOrWhiteSpace(aircraft.Modelo))
            {
                return "El modelo de la aeronave es obligatorio.";
            }

            if (aircraft.MaxWeight <= 0)
            {
                return "El peso máximo debe ser mayor a 0.";
            }

            if (
                aircraft.Cant_Asientos_Fila_Firstclass < 0 ||
                aircraft.Cant_Filas_Firstclass < 0 ||
                aircraft.Cant_Asientos_Fila_Turista < 0 ||
                aircraft.Cant_Filas_Turista < 0
            )
            {
                return "Las filas y asientos no pueden ser negativos.";
            }

            if (aircraft.AircraftSize == "No definido")
            {
                return "Debe seleccionar el tamaño de la aeronave.";
            }

            return string.Empty;
        }
    }
}
