using System.Data;
using Dapper;
using AirDreams.API.Models;
using AirDreams.API.DTOs;

public class RouteRepository : IRouteRepository
{
    private readonly IDbConnection _connection;

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
    a1.codeAirport AS DepCode,
    a1.nameAirport AS DepName,
    a1.city AS DepCity,
    a1.country AS DepCountry,
    a1.timeZone AS DepTimeZone,
    a2.codeAirport AS ArrCode,
    a2.nameAirport AS ArrName,
    a2.city AS ArrCity,
    a2.country AS ArrCountry,
    a2.timeZone AS ArrTimeZone
    FROM Route r
    LEFT JOIN Airport a1 ON r.codeAirportSalida = a1.codeAirport
    LEFT JOIN Airport a2 ON r.codeAirportLlegada = a2.codeAirport;
    ";

    
    var result = await _connection.QueryAsync<RouteDTO, AirportDTO, AirportDTO, RouteDTO>(
        sql,
        (route, dep, arr) =>
        {
            route.DepartureAirport = dep ?? new AirportDTO { Code = (route.DepartureAirport?.Code ?? ""), Name = (route.DepartureAirport?.Name ?? "") };
            route.ArrivalAirport = arr ?? new AirportDTO { Code = (route.ArrivalAirport?.Code ?? ""), Name = (route.ArrivalAirport?.Name ?? "") };
            return route;
        },
        splitOn: "DepCode,ArrCode"
    );

    // Dapper no mapeará automáticamente AirportDTO propiedades si los alias no coinciden; para asegurarlo, puedes reconstruir dep/arr:
    var list = result.ToList();
    // si deseas formatear Duration o nombres, hazlo aquí
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
                    firstClassPrice, turistClassPrice, stimatedTime, distance)
                VALUES (@AdminID,@CodeAirportSalida,@CodeAirportLlegada,@PlateNumber,
                    @FirstClassPrice,@TuristClassPrice,@StimatedTime,@Distance);
                SELECT CAST(SCOPE_IDENTITY() AS int);
            ";
            var routeId = await _connection.ExecuteScalarAsync<int>(insertRoute, model, tran);

            var insertFreq = @"
                INSERT INTO FlightFrequency (idRoute, dayOfWeek, departureTime, estimatedArrivalTime, startingDate, endingDate, active)
                VALUES (@IdRoute, @DayOfWeek, @DepartureTime, @EstimatedArrivalTime, @StartingDate, @EndingDate, @Active);
            ";

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