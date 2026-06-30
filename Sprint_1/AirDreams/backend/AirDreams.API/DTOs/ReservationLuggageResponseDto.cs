namespace AirDreams.API.DTOs
{
    public class ReservationLuggageResponseDto
    {
        public IEnumerable<ReservationLuggageDto> Luggage { get; set; } =
            new List<ReservationLuggageDto>();

        public IEnumerable<ReservationLuggageSegmentDto> Segments { get; set; } =
            new List<ReservationLuggageSegmentDto>();
    }
}