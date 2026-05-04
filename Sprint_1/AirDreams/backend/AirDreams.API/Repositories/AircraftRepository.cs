using AirDreams.API.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace AirDreams.API.Repositories

{
    public class AircraftRepository
    {
        private readonly string _connectionString;

        public AircraftRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        //Obtener todos los aviones
        public List<AircraftModel> GetAircrafts()
        {
            using var connection = new SqlConnection(_connectionString);
            string query = "SELECT * FROM Aircraftt";

            return connection.Query<AircraftModel>(query).ToList();
        }

        //Agregar aeronave
        public bool AddAircraft(AircraftModel aircraft)
        {
            using var connection = new SqlConnection(_connectionString);

            string query = @"
                INSERT INTO dbo.Aircraftt
                    (plateNumber, maxWeight, cantPasajeros,
                    cant_Asientos_Fila_Firstclass, cant_Filas_Firstclass,
                    cant_Asientos_Fila_Turista, cant_Filas_Turista, modelo)
                VALUES
                    (@plateNumber, @maxWeight, @cantPasajeros,
                    @cant_Asientos_Fila_Firstclass, @cant_Filas_Firstclass,
                    @cant_Asientos_Fila_Turista, @cant_Filas_Turista, @modelo)";

            var affectedRows = connection.Execute(query, aircraft);
            return affectedRows > 0;
        }
    

        //Eliminar aeronave
        public bool DeleteAircraft(string plateNumber)
        {
            using var connection = new SqlConnection(_connectionString);

            string query = "DELETE FROM Aircraftt WHERE plateNumber = @plateNumber";

            var affectedRows = connection.Execute(query, new { plateNumber });

            return affectedRows > 0;

        }


        public AircraftModel? GetAircraftByPlateNumber(string plateNumber)
        {
            using var connection = new SqlConnection(_connectionString);

            string query = @"
        SELECT *
        FROM dbo.Aircraftt
        WHERE plateNumber = @plateNumber";

            return connection.QueryFirstOrDefault<AircraftModel>(
                query,
                new { plateNumber }
            );
        }

        public bool UpdateAircraft(AircraftModel aircraft)
        {
            using var connection = new SqlConnection(_connectionString);

            string query = @"
        UPDATE dbo.Aircraftt
        SET 
            maxWeight = @maxWeight,
            cantPasajeros = @cantPasajeros,
            cant_Asientos_Fila_Firstclass = @cant_Asientos_Fila_Firstclass,
            cant_Filas_Firstclass = @cant_Filas_Firstclass,
            cant_Asientos_Fila_Turista = @cant_Asientos_Fila_Turista,
            cant_Filas_Turista = @cant_Filas_Turista,
            modelo = @modelo
        WHERE plateNumber = @plateNumber";

            var affectedRows = connection.Execute(query, aircraft);

            return affectedRows > 0;
        }
    }
}