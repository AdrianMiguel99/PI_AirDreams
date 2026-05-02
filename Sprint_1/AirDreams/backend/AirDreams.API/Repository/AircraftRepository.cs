using AirDreams.API.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace AirDreams.API.Repository
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
        public void AddAircraft(AircraftModel aircraft)
        {
            using var connection = new SqlConnection(_connectionString);

            string query = @"
        INSERT INTO dbo.Aircraftt
        (plateNumber, maxWeight, cantPasajeros, modelo,
         cant_Asientos_Fila_Firstclass, cant_Filas_Firstclass,
         cant_Asientos_Fila_Turista, cant_Filas_Turista)
        VALUES
        (@matricula, @maxWeight, @cantPasajeros, @modelo,
         @cant_Asientos_Fila_Firstclass, @cant_Filas_Firstclass,
         @cant_Asientos_Fila_Turista, @cant_Filas_Turista)";

            connection.Execute(query, aircraft);
        }
    }
}