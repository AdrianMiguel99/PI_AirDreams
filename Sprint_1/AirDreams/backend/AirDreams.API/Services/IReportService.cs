using AirDreams.API.DTOs;

namespace AirDreams.API.Services
{
    public interface IReportService
    {
        Task<IEnumerable<IncomeReportDTO>> GetIncomeReportAsync(
            int? year,
            DateTime? startDate,
            DateTime? endDate,
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
