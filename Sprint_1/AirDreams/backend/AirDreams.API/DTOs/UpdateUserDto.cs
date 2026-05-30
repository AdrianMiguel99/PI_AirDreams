namespace AirDreams.API.DTOs
{
    public class UpdateUserDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsAdmin { get; set; }
        public bool? IsOperator { get; set; }
    }
}