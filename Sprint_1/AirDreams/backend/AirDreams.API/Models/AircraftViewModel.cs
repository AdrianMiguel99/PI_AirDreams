namespace AirDreams.API.Models
{
    public class AircraftViewModel
    {
        public int AdminID { get; set; }
        public string Modelo { get; set; } = string.Empty;
        public string AircraftSize { get; set; } = string.Empty;
        public int MaxWeight { get; set; }

        public int Cant_Asientos_Fila_Firstclass { get; set; }
        public int Cant_Filas_Firstclass { get; set; }

        public int Cant_Asientos_Fila_Turista { get; set; }
        public int Cant_Filas_Turista { get; set; }

        public int CantPasajeros { get; set; }
    }
}
