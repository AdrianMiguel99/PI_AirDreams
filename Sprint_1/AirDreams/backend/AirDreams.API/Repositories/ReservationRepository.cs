using AirDreams.API.DTOs;
using AirDreams.API.Repositories;
using Dapper;
using System.Data;

public class ReservationRepository : IReservationRepository
{
    private readonly IDbConnection _connection;

    public ReservationRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<ReservationDetailsDTO?> GetReservationDetailsAsync(string reservationCode)
    {
        if (_connection.State == ConnectionState.Closed)
            _connection.Open();

        var parameters = new
        {
            ReservationCode = reservationCode
        };

        using var result = await _connection.QueryMultipleAsync(
            "dbo.GetReservationDetails",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        var passengers = (await result.ReadAsync<ReservationPassengerDTO>()).ToList();
        var flights = (await result.ReadAsync<ReservationFlightDTO>()).ToList();

        if (!passengers.Any() && !flights.Any())
        {
            return null;
        }

        return new ReservationDetailsDTO
        {
            Passengers = passengers,
            Flights = flights
        };
    }
}