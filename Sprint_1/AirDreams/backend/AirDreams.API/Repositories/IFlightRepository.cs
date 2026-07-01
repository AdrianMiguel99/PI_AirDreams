namespace AirDreams.API.Repositories
{
    public interface IFlightRepository
    {
        Task<IEnumerable<dynamic>> SearchFlightsAsync(
            string origin,
            string destination,
            DateTime earliestTime,
            DateTime latestTime,
            int quantityOfPassengers
        );

        Task<IEnumerable<dynamic>> SearchOneStopFlightsAsync(
            string origin,
            string destination,
            DateTime earliestTime,
            DateTime latestTime,
            int quantityOfPassengers
        );

        Task<IEnumerable<dynamic>> SearchFlightsByDestinationAsync(
            string destination,
            DateTime earliestTime,
            DateTime latestTime,
            int quantityOfPassengers
        );

        Task<IEnumerable<dynamic>> SearchFlightsByOriginAsync(
            string origin,
            DateTime earliestDeparture,
            DateTime latestDeparture,
            int quantityOfPassengers
        );

        Task<dynamic> GetFlightByGuidAsync(string flightGuid);
        Task<int> GetRouteIdByFlightGuidAsync(string flightGuid);
    }
}