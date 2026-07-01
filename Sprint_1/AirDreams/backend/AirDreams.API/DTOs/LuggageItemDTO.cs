using System.ComponentModel.DataAnnotations;

namespace AirDreams.API.DTOs
{

    public class LuggageItemDTO
    {
        public string Type { get; set; }        
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }
    }

}