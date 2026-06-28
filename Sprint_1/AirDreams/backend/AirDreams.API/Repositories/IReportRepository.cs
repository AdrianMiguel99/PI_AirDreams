using AirDreams.API.DTOs;

namespace AirDreams.API.Repositories
{
    public interface IReportRepository
    {
        Task<IEnumerable<IncomeReportDTO>> GetIncomeReportAsync(
            DateTime startDate,
            DateTime endDate,
            string? origin,
            string? destination);

        Task<IncomeReportFiltersDTO> GetIncomeReportFiltersAsync();

        Task<IEnumerable<FlightsReportRowDto>> GetFlightsReportAsync(
            string? origin,
            string? destination,
            string? seatClass,
            DateTime? fromDate,
            DateTime? toDate);
        Task<FlightsReportFiltersDto> GetFlightsReportFiltersAsync();
    }
}
