namespace AirDreams.API.Repositories
{
    public interface ILuggageCalculationRepository
    {
        Task<decimal> CalculateAsync(decimal basePrice, decimal multiplier, int quantity);
    }
}