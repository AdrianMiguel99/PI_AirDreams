using AirDreams.API.Models;

namespace AirDreams.API.Services.Interfaces
{
    public interface IAircraftService
    {
        List<AircraftModel> GetAircrafts();
        string AddAircraft(AircraftModel aircraft);
        string DeleteAircraft(string plateNumber);
        AircraftModel? GetAircraftByPlateNumber(string plateNumber);
        string UpdateAircraft(AircraftModel aircraft);
    }
}

