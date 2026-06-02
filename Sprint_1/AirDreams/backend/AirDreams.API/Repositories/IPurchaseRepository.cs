using AirDreams.API.Models.Dtos;

namespace AirDreams.API.Repositories
{
    public interface IPurchaseRepository
    {
        Task ConfirmPurchaseAsync(ConfirmPurchaseDto dto, string? cardLastFour);
    }
}