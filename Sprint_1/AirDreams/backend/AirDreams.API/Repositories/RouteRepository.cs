using backend.Models;
using Dapper;
using System.Data.SqlClient;

namespace backend.Repositories
{
    public class RouteRepository
    {
        private readonly string _connectionString;

        public RouteRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("AirDreamsContext");
        }

        public bool CreateRoute(CreateRouteModel route)
        {
            using var connection = new SqlConnection(_connectionString);

            string query = @"
                INSERT INTO Route
                (
                    adminID,
                    codeAirportSalida,
                    codeAirportLlegada,
                    plateNumber,
                    firstClassPrice,
                    turistClassPrice,
                    routeState,
                    stimatedTime,
                    distance
                )
                VALUES
                (
                    @AdminID,
                    @CodeAirportSalida,
                    @CodeAirportLlegada,
                    @PlateNumber,
                    @FirstClassPrice,
                    @TuristClassPrice,
                    @RouteState,
                    @StimatedTime,
                    @Distance
                );
            ";

            int affectedRows = connection.Execute(query, route);

            return affectedRows >= 1;
        }
    }
}