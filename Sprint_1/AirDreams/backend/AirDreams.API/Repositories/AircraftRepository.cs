using System.Data;
using AirDreams.API.Models;
using Dapper;

namespace AirDreams.API.Repositories
{
    public class AircraftRepository
    {
        private readonly IDbConnection _connection;

        public AircraftRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public List<AircraftViewModel> GetAircrafts()
        {
            string query = @"
                SELECT *
                FROM AircraftView";

            return _connection
                .Query<AircraftViewModel>(query)
                .ToList();
        }

        public bool AddAircraft(AircraftModel aircraft)
        {
            string query = @"
                INSERT INTO dbo.Aircraft
                (
                    adminId,
                    modelo,
                    aircraftSize,
                    maxWeight,
                    cant_Asientos_Fila_Firstclass,
                    cant_Filas_Firstclass,
                    cant_Asientos_Fila_Turista,
                    cant_Filas_Turista
                )
                VALUES
                (
                    @adminId,
                    @modelo,
                    @aircraftSize,
                    @maxWeight,
                    @cant_Asientos_Fila_Firstclass,
                    @cant_Filas_Firstclass,
                    @cant_Asientos_Fila_Turista,
                    @cant_Filas_Turista
                )";

            int affectedRows =
                _connection.Execute(query, aircraft);

            return affectedRows > 0;
        }

        public bool DeleteAircraft(string modelo)
        {
            string query = @"
                DELETE FROM Aircraft
                WHERE modelo = @modelo";

            int affectedRows =
                _connection.Execute(
                    query,
                    new { modelo });

            return affectedRows > 0;
        }

        public AircraftViewModel? GetAircraftByModel(string modelo)
        {
            string query = @"
                SELECT *
                FROM AircraftView
                WHERE modelo = @modelo";

            return _connection.QueryFirstOrDefault<AircraftViewModel>(
                query,
                new { modelo }
            );
        }

        public bool UpdateAircraft(AircraftModel aircraft)
        {
            string query = @"
                UPDATE dbo.Aircraft
                SET
                    aircraftSize = @aircraftSize,
                    maxWeight = @maxWeight,

                    cant_Asientos_Fila_Firstclass =
                        @cant_Asientos_Fila_Firstclass,

                    cant_Filas_Firstclass =
                        @cant_Filas_Firstclass,

                    cant_Asientos_Fila_Turista =
                        @cant_Asientos_Fila_Turista,

                    cant_Filas_Turista =
                        @cant_Filas_Turista

                WHERE modelo = @modelo";

            int affectedRows =
                _connection.Execute(
                    query,
                    aircraft
                );

            return affectedRows > 0;
        }

        public bool ExistsByModel(string modelo)
        {
            string query = @"
            SELECT COUNT(*)
            FROM Aircraft
            WHERE modelo = @modelo";

            return _connection.ExecuteScalar<int>(
                query,
                new { modelo }
            ) > 0;
        }

        public bool IsAircraftInUse(string modelo)
        {
            string query = @"
            SELECT COUNT(*)
            FROM Route
            WHERE modelo = @modelo";

            return _connection.ExecuteScalar<int>(
                query,
                new { modelo }
            ) > 0;
        }
    }
}