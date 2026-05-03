namespace AirDreams.API.Models
{
    public class RegisterUserModel
    {
        public string NombreCompleto { get; set; }

        public string TipoUsuario { get; set; }

        public string Correo { get; set; }

        public string Cedula { get; set; }

        public bool emailConfirmed { get; set; }
    }
}