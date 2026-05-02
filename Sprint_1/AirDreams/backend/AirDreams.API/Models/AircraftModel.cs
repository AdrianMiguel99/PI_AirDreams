namespace AirDreams.API.Models
{
    public class AircraftModel
    {
        public string matricula { get; set; }
        public int maxWeight { get; set; }
        public int cantPasajeros { get; set; }

        public int cant_Asientos_Fila_Firstclass { get; set; }

        public int cant_Filas_Firstclass { get; set; }

        public int cant_Asientos_Fila_Turista { get; set; }

        public int cant_Filas_Turista { get; set; }

        public string modelo { get; set; }

    }
}