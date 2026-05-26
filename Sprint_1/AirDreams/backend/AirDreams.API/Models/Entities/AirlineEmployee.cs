namespace AirDreams.API.Models
{
    public class AirlineEmployee
    {
        public byte EmployeeID { get; set; }
        public string EmailInternalUser { get; set; } = string.Empty;
        public string Lastnames { get; set; } = string.Empty;
        public string NameEmployee { get; set; } = string.Empty;
        public bool IsAdmin { get; set; } 
        public bool IsOperator { get; set; }
    }
}