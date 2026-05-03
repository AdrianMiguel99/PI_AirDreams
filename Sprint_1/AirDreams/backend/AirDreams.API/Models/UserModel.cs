namespace AirDreams.API.Models
{
    public class UserModel
    {
        public int Id { get; set; }

        public string NombreCompleto { get; set; }

        public string TipoUsuario { get; set; }

        public string Correo { get; set; }

        public string Cedula { get; set; }

        public string PasswordHash { get; set; }

        public bool emailConfirmed { get; set; }
    }
}