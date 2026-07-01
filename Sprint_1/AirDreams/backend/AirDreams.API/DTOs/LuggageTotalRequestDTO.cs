using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace AirDreams.API.DTOs

{
    public class LuggageTotalRequestDTO
    {
        [Required] public int CheckedQuantity { get; set; }
        [Required] public int CarryOnQuantity { get; set; }
        [Required]
        public List<SegmentPricingDTO> Segments { get; set; } 
    }

    public class SegmentPricingDTO
    {
        public decimal CheckedPrice { get; set; }
        public decimal CarryOnPrice { get; set; }
        public decimal Multiplier { get; set; }
    }
}