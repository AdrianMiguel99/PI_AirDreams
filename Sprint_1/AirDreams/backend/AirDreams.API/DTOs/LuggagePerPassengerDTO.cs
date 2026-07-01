using System.ComponentModel.DataAnnotations;

namespace AirDreams.API.DTOs
{


    public class LuggagePerPassengerDto
    {
        public int PassengerIndex { get; set; }
        public List<LuggageItemDTO> LuggageItems { get; set; }
    }

};