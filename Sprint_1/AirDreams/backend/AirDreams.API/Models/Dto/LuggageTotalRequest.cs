using System.ComponentModel.DataAnnotations;

namespace AirDreams.API.Models.Dtos
{
    public class LuggageTotalRequest
    {
        [Required] public int CheckedQuantity { get; set; }
        [Required] public int CarryOnQuantity { get; set; }
        [Required] public List<SegmentPricing> Segments { get; set; }
    }

    public class SegmentPricing
    {
        public decimal CheckedPrice { get; set; }
        public decimal CarryOnPrice { get; set; }
        public decimal Multiplier { get; set; }
    }
}