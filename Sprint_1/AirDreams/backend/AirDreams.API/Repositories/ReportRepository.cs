using System.Data;
using AirDreams.API.DTOs;
using Dapper;

namespace AirDreams.API.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly IDbConnection _connection;

        public ReportRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<IncomeReportDTO>> GetIncomeReportAsync(
            DateTime startDate,
            DateTime endDate,
            string? origin,
            string? destination,
            string? airline)
        {
            return await _connection.QueryAsync<IncomeReportDTO>(
                "dbo.sp_GetIncomeReport",
                new
                {
                    FechaInicio = startDate.Date,
                    FechaFinExclusiva = endDate.Date,
                    Origen = string.IsNullOrWhiteSpace(origin) ? null : origin.Trim().ToUpper(),
                    Destino = string.IsNullOrWhiteSpace(destination) ? null : destination.Trim().ToUpper(),
                    Aerolinea = string.IsNullOrWhiteSpace(airline) ? null : airline.Trim()
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<IncomeReportFiltersDTO> GetIncomeReportFiltersAsync()
        {
            using var results = await _connection.QueryMultipleAsync(
                "dbo.sp_GetIncomeReportFilters",
                commandType: CommandType.StoredProcedure);

            var origins = await results.ReadAsync<string>();
            var destinations = await results.ReadAsync<string>();
            var years = await results.ReadAsync<int>();
            var airlines = await results.ReadAsync<string>();

            return new IncomeReportFiltersDTO
            {
                Origins = origins,
                Destinations = destinations,
                Years = years,
                Airlines = airlines
            };
        }
        public async Task<IEnumerable<FlightsReportRowDto>> GetFlightsReportAsync(
            string? origin,
            string? destination,
            string? seatClass,
            DateTime? fromDate,
            DateTime? toDate)
        {
            return await _connection.QueryAsync<FlightsReportRowDto>(
                "dbo.sp_GetFlightsReport",
                new
                {
                    Origin = string.IsNullOrWhiteSpace(origin) ? null : origin.Trim().ToUpper(),
                    Destination = string.IsNullOrWhiteSpace(destination) ? null : destination.Trim().ToUpper(),
                    SeatClass = seatClass, 
                    FromDate = fromDate?.Date,
                    ToDate = toDate?.Date
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<FlightsReportFiltersDto> GetFlightsReportFiltersAsync()
        {
            using var results = await _connection.QueryMultipleAsync(
                "dbo.sp_GetFlightsReportFilters",
                commandType: CommandType.StoredProcedure);

            var origins = await results.ReadAsync<string>();
            var destinations = await results.ReadAsync<string>();
            var range = await results.ReadSingleOrDefaultAsync();

            return new FlightsReportFiltersDto
            {
                Origins = origins,
                Destinations = destinations,
                MinDate = range?.minDate,
                MaxDate = range?.maxDate
            };
        }
    }
}
