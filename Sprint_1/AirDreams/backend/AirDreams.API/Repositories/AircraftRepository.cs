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

        public List<AircraftModel> GetAircrafts()
        {
            string query = "SELECT * FROM Aircraft";
            return _connection.Query<AircraftModel>(query).ToList();
        }

        public bool AddAircraft(AircraftModel aircraft)
        {
            string query = @"
                INSERT INTO dbo.Aircraft
                    (plateNumber, maxWeight, cantPasajeros,
                     cant_Asientos_Fila_Firstclass, cant_Filas_Firstclass,
                     cant_Asientos_Fila_Turista, cant_Filas_Turista,
                     modelo, adminId)
                VALUES
                    (@plateNumber, @maxWeight, @cantPasajeros,
                     @cant_Asientos_Fila_Firstclass, @cant_Filas_Firstclass,
                     @cant_Asientos_Fila_Turista, @cant_Filas_Turista,
                     @modelo, @adminId)";

            var affectedRows = _connection.Execute(query, aircraft);
            return affectedRows > 0;
        }

        public bool DeleteAircraft(string plateNumber)
        {
            string query = "DELETE FROM Aircraft WHERE plateNumber = @plateNumber";

            var affectedRows = _connection.Execute(query, new { plateNumber });

            return affectedRows > 0;
        }

        public AircraftModel? GetAircraftByPlateNumber(string plateNumber)
        {
            string query = @"
                SELECT *
                FROM dbo.Aircraft
                WHERE plateNumber = @plateNumber";

            return _connection.QueryFirstOrDefault<AircraftModel>(
                query,
                new { plateNumber }
            );
        }

        public bool UpdateAircraft(AircraftModel aircraft)
        {
            string query = @"
                UPDATE dbo.Aircraft
                SET 
                    maxWeight = @maxWeight,
                    cantPasajeros = @cantPasajeros,
                    cant_Asientos_Fila_Firstclass = @cant_Asientos_Fila_Firstclass,
                    cant_Filas_Firstclass = @cant_Filas_Firstclass,
                    cant_Asientos_Fila_Turista = @cant_Asientos_Fila_Turista,
                    cant_Filas_Turista = @cant_Filas_Turista,
                    modelo = @modelo
                WHERE plateNumber = @plateNumber";

            var affectedRows = _connection.Execute(query, aircraft);

            return affectedRows > 0;
        }
    }
}