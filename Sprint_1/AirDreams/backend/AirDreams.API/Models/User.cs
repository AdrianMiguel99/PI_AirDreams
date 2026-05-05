namespace AirDreams.API.Models
{
    public class User
    {
        public byte UserId { get; set; }
        public string Email { get; set; }
        public byte EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsOperator { get; set; }
    }
}
