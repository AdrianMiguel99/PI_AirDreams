using System.Data;
using Dapper;
using AirDreams.API.Models;

public class RouteRepository : IRouteRepository
{
    private readonly IDbConnection _connection;

    public RouteRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<CreateRouteModel>> GetAllAsync()
    {
        const string sql = @"
        SELECT r.idRoute AS Id, r.plateNumber AS PlateNumber,
                r.turistClassPrice AS TouristPrice, r.firstClassPrice AS FirstClassPrice,
                r.stimatedTime AS StimatedTime,
                r.codeAirportSalida AS DepartureCode, a1.nameAirport AS DepartureName,
                r.codeAirportLlegada AS ArrivalCode, a2.nameAirport AS ArrivalName
        FROM Route r
        LEFT JOIN Airport a1 ON r.codeAirportSalida = a1.codeAirport
        LEFT JOIN Airport a2 ON r.codeAirportLlegada = a2.codeAirport
        ";
        
        var routes = await _connection.QueryAsync<CreateRouteModel>(sql);
        return routes;
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