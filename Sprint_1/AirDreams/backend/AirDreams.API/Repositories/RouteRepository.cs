using System.Data;
using Dapper;
using AirDreams.API.Models;
using AirDreams.API.DTOs;

public class RouteRepository : IRouteRepository
{
    private readonly IDbConnection _connection;

    private static DateTime GetNextDateForDay(DateTime startDate, string dayOfWeek)
    {
        var targetDay = Enum.Parse<DayOfWeek>(dayOfWeek, true);

        var daysToAdd = ((int)targetDay - (int)startDate.DayOfWeek + 7) % 7;

        return startDate.AddDays(daysToAdd);
    }

    public RouteRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<RouteDTO>> GetAllAsync()
    {
    if (_connection.State == ConnectionState.Closed) _connection.Open();

    var sql = @"
    SELECT
    r.idRoute AS Id,
    r.stimatedTime AS Duration,
    r.firstClassPrice AS FirstClassPrice,
    r.turistClassPrice AS TouristPrice,
    a1.codeAirport AS Code,
    a1.nameAirport AS Name,
    a1.city AS City,
    a1.country AS Country,
    a1.timeZone AS TimeZone,
    a2.codeAirport AS Code,
    a2.nameAirport AS Name,
    a2.city AS City,
    a2.country AS Country,
    a2.timeZone AS TimeZone
    FROM Route r
    LEFT JOIN Airport a1 ON r.codeAirportSalida = a1.codeAirport
    LEFT JOIN Airport a2 ON r.codeAirportLlegada = a2.codeAirport;
    ";

    
    var result = await _connection.QueryAsync<RouteDTO, AirportDTO, AirportDTO, RouteDTO>(
        sql,
        (route, dep, arr) =>
        {
            route.DepartureAirport = dep;
            route.ArrivalAirport = arr;
            return route;
        },
        splitOn: "Code,Code"
    );


    var list = result.ToList();

    return list;
    }

    public async Task<int> CreateRouteWithFrequenciesAsync(CreateRouteModel model)
    {
        if (_connection.State == ConnectionState.Closed) _connection.Open();
        using var tran = _connection.BeginTransaction();
        try
        {
            var insertRoute = @"
                INSERT INTO Route (adminID, codeAirportSalida, codeAirportLlegada, plateNumber,
                    firstClassPrice, turistClassPrice, routeState, stimatedTime, distance)
                VALUES (@AdminID,@CodeAirportSalida,@CodeAirportLlegada,@PlateNumber,
                    @FirstClassPrice,@TuristClassPrice, @RouteState, @StimatedTime,@Distance);
                SELECT CAST(SCOPE_IDENTITY() AS int);
            ";
            var routeId = await _connection.ExecuteScalarAsync<int>(insertRoute, model, tran);

            var insertFreq = @"
                INSERT INTO FlightFrequency (idRoute, dayOfWeek, departureTime, estimatedArrivalTime, startingDate, endingDate, active)
                VALUES (@IdRoute, @DayOfWeek, @DepartureTime, @EstimatedArrivalTime, @StartingDate, @EndingDate, @Active);
            ";

            var insertFlight = @"
                INSERT INTO Flight (
                    numberFlight,
                    routeId,
                    boardingGate,
                    priceLuggage,
                    departureDate
                    )
                VALUES (
                    @NumberFlight,
                    @RouteId,
                    @BoardingGate,
                    @PriceLuggage,
                    @DepartureDate
                );
            ";

            int index = 1;

            foreach (var f in model.Frequencies)
            {
                var p = new {
                    IdRoute = routeId,
                    DayOfWeek = f.DayOfWeek,
                    DepartureTime = f.DepartureTime,
                    EstimatedArrivalTime = f.EstimatedArrivalTime,
                    StartingDate = f.StartingDate.Date,
                    EndingDate = f.EndingDate.Date,
                    Active = f.Active ? 1 : 0
                };
                await _connection.ExecuteAsync(insertFreq, p, tran);

                var flight = new {
                    NumberFlight = $"F{routeId:D3}{index:D2}",
                    RouteId = routeId,
                    BoardingGate = 1,
                    PriceLuggage = model.PriceLuggage,
                    DepartureDate = GetNextDateForDay(f.StartingDate.Date, f.DayOfWeek)
                };

                await _connection.ExecuteAsync(insertFlight, flight, tran);

                index++;
            }

            tran.Commit();
            return routeId;
        }
        catch
        {
            tran.Rollback();
            throw;
        }
    }
}