using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace AirDreams.API.Models.Entities

{
    public class Passenger
    {
        [Key]
        public int IdPassenger { get; set; }
        public string NamePassenger { get; set; } = string.Empty;
        public string LastnamesPassenger { get; set; } = string.Empty;
        public string EmailPassenger { get; set; } = string.Empty;
        public long? Telephone { get; set; }
        public byte? CountryCode { get; set; }
    }
}
