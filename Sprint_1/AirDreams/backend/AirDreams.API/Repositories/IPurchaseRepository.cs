using AirDreams.API.Models.Dtos;

namespace AirDreams.API.Repositories
{
    public interface IPurchaseRepository
    {
        Task ConfirmPurchaseAsync(ConfirmPurchaseDto dto, decimal amount, string? cardLastFour);
    }
}