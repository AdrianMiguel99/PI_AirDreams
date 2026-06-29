using AirDreams.API.DTOs;
using AirDreams.API.Models;

namespace AirDreams.API.Services.Interfaces
{
    public interface IAircraftService
    {
        List<AircraftViewModel> GetAircrafts();
        string AddAircraft(AircraftModel aircraft);
        AircraftDeleteResultDTO DeleteAircraft(string modelo);
        AircraftViewModel? GetAircraftByModel(string modelo);
        string UpdateAircraft(AircraftModel aircraft);
    }
}
