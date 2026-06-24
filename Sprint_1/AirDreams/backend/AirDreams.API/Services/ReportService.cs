using AirDreams.API.DTOs;
using AirDreams.API.Repositories;

namespace AirDreams.API.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;

        public ReportService(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        public async Task<IEnumerable<IncomeReportDTO>> GetIncomeReportAsync(
            int? year,
            DateTime? startDate,
            DateTime? endDate,
            string? origin,
            string? destination)
        {
            var resolvedStartDate = year.HasValue
                ? new DateTime(year.Value, 1, 1)
                : startDate?.Date ?? new DateTime(1900, 1, 1);

            var resolvedEndDate = year.HasValue
                ? new DateTime(year.Value + 1, 1, 1)
                : endDate?.Date.AddDays(1) ?? new DateTime(9999, 12, 31);

            if (resolvedEndDate <= resolvedStartDate)
                throw new ArgumentException("La fecha final debe ser mayor que la fecha inicial.");

            return await _reportRepository.GetIncomeReportAsync(
                resolvedStartDate,
                resolvedEndDate,
                origin,
                destination);
        }

        public async Task<IncomeReportFiltersDTO> GetIncomeReportFiltersAsync()
        {
            return await _reportRepository.GetIncomeReportFiltersAsync();
        }
    }
}
