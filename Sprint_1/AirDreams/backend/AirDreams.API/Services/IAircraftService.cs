using AirDreams.API.Models;

namespace AirDreams.API.Services.Interfaces
{
    public interface IAircraftService
    {
        List<AircraftViewModel> GetAircrafts();
        string AddAircraft(AircraftModel aircraft);
        string DeleteAircraft(string modelo);
        AircraftViewModel? GetAircraftByModel(string modelo);
        string UpdateAircraft(AircraftModel aircraft);
    }
}

