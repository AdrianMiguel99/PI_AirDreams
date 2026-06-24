namespace AirDreams.API.Repositories
{
    public interface IPassengerValidationRepository
    {
        Task<IEnumerable<string>> FindDuplicatePassengersAsync(
            IEnumerable<PassengerCheck> passengers,
            IEnumerable<string> flightNumbers);
    }

    public class PassengerCheck
    {
        public string NamePassenger { get; set; }
        public string LastnamesPassenger { get; set; }
        public string Country { get; set; }
        public string? BirthDate { get; set; }
    }
}