using AirDreams.API.DTOs;

namespace AirDreams.API.Repositories
{
    public interface IReportRepository
    {
        Task<IEnumerable<IncomeReportDTO>> GetIncomeReportAsync(
            DateTime startDate,
            DateTime endDate,
            string? origin,
            string? destination,
            string? airline);

        Task<IncomeReportFiltersDTO> GetIncomeReportFiltersAsync();

        Task<IEnumerable<FlightsReportRowDTO>> GetFlightsReportAsync(
            string? origin,
            string? destination,
            string? seatClass,
            DateTime? fromDate,
            DateTime? toDate);
        Task<FlightsReportFiltersDTO> GetFlightsReportFiltersAsync();
    }
}
