using System.Data;
using AirDreams.API.DTOs;
using AirDreams.API.Models;
using Dapper;

namespace AirDreams.API.Repositories
{
    public class AircraftRepository : IAircraftRepository
    {
        private readonly IDbConnection _connection;

        public AircraftRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public List<AircraftViewModel> GetAircrafts()
        {
            string query = @"
                SELECT av.*
                FROM AircraftView av
                INNER JOIN Aircraft a ON av.modelo = a.modelo
                WHERE a.isDeleted = 0";

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

        public AircraftDeleteResultDTO DeleteAircraft(string modelo)
        {
            var result = _connection.QuerySingle<AircraftDeleteResultDTO>(
                "dbo.sp_DeleteAircraft",
                new { Modelo = modelo },
                commandType: CommandType.StoredProcedure
            );

            return result;
        }

        public AircraftViewModel? GetAircraftByModel(string modelo)
        {
            string query = @"
                SELECT av.*
                FROM AircraftView av
                INNER JOIN Aircraft a ON av.modelo = a.modelo
                WHERE av.modelo = @modelo
                    AND a.isDeleted = 0";

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

                WHERE modelo = @modelo
                    AND isDeleted = 0";

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
