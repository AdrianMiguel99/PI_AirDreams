using System.ComponentModel.DataAnnotations;

namespace AirDreams.API.Models
{
    public class LuggageRegistrationModel
    {
        public int? IdPassenger { get; set; }

        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "ID de itinerario es requerido")]
        public string TransactionIdItinerary { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe proporcionar al menos un equipaje")]
        [MinLength(1, ErrorMessage = "Debe proporcionar al menos un equipaje")]
        public List<LuggageItemModel> LuggageItems { get; set; } = new List<LuggageItemModel>();
    }

    public class LuggageItemModel
    {
        [Required(ErrorMessage = "Tipo de equipaje es requerido")]
        public string Type { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Cantidad de equipaje debe ser mayor a 0")]
        public int Quantity { get; set; }
    }
}
