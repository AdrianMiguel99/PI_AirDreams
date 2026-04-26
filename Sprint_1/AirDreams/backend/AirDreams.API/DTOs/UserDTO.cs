namespace AirDreams.API.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public bool isOperator { get; set; }
    }
}