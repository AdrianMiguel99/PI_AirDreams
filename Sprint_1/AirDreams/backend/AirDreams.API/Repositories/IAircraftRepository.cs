using AirDreams.API.DTOs;
using AirDreams.API.Models;

namespace AirDreams.API.Repositories
{
    public interface IAircraftRepository
    {
        List<AircraftViewModel> GetAircrafts();
        bool AddAircraft(AircraftModel aircraft);
        AircraftDeleteResultDTO DeleteAircraft(string modelo);
        AircraftViewModel? GetAircraftByModel(string modelo);
        bool UpdateAircraft(AircraftModel aircraft);
        bool ExistsByModel(string modelo);
        bool IsAircraftInUse(string modelo);
    }
}
