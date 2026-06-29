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

    public async Task<ReservationDetailsDto?> GetReservationDetailsAsync(string reservationCode)
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

        var passengers = (await result.ReadAsync<ReservationPassengerDto>()).ToList();
        var flights = (await result.ReadAsync<ReservationFlightDto>()).ToList();

        if (!passengers.Any() && !flights.Any())
        {
            return null;
        }

        return new ReservationDetailsDto
        {
            Passengers = passengers,
            Flights = flights
        };
    }
}