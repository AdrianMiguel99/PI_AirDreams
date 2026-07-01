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
        LEFT JOIN Airport a2 ON r.codeAirportLlegada = a2.codeAirport
        WHERE r.isDeleted = 0;
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
                INSERT INTO Route (
                adminID,
                codeAirportSalida, 
                codeAirportLlegada, 
                modelo,
                firstClassPrice,
                turistClassPrice,
                stimatedTime,
                luggagePrice,
                luggageMaxWeight,
                carryOnPrice,
                carryOnMaxWeight,
                porcentageMultiplier,
                distance,
                isDeleted
                )

                VALUES (
                @AdminID,
                @CodeAirportSalida,
                @CodeAirportLlegada,
                @Modelo,
                @FirstClassPrice,
                @TuristClassPrice,
                @StimatedTime,
                @luggagePrice,
                @luggageMaxWeight,
                @carryOnPrice,
                @carryOnMaxWeight,
                @porcentageMultiplier,             
                @Distance,
                0);

                SELECT CAST(SCOPE_IDENTITY() AS int);
            ";
            var routeId = await _connection.ExecuteScalarAsync<int>(insertRoute, model, tran);

            var insertFreq = @"
                INSERT INTO FlightFrequency (
                idRoute,
                dayOfWeek,
                departureTime,
                estimatedArrivalTime,
                startingDate,
                endingDate,
                active)

                VALUES (
                @IdRoute,
                @DayOfWeek,
                @DepartureTime,
                @EstimatedArrivalTime,
                CAST(GETDATE() AS DATE),
                @EndingDate,
                @Active);
            ";


            int index = 1;

            foreach (var f in model.Frequencies)
            {
                var p = new {
                    IdRoute = routeId,
                    DayOfWeek = f.DayOfWeek,
                    DepartureTime = f.DepartureTime,
                    EstimatedArrivalTime = f.EstimatedArrivalTime,
                    EndingDate = f.EndingDate.Date,
                    Active = f.Active ? 1 : 0
                };
                await _connection.ExecuteAsync(insertFreq, p, tran);

            

            
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

    public async Task<RouteDeleteResultDTO> DeleteAsync(int routeId)
    {
        if (_connection.State == ConnectionState.Closed) _connection.Open();

        var result = await _connection.QuerySingleAsync<RouteDeleteResultDTO>(
            "dbo.sp_DeleteRoute",
            new { RouteId = routeId },
            commandType: CommandType.StoredProcedure
        );

        return result;
    }
}
