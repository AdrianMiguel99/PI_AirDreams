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
            string? destination)
        {
            return await _connection.QueryAsync<IncomeReportDTO>(
                "dbo.sp_GetIncomeReport",
                new
                {
                    FechaInicio = startDate.Date,
                    FechaFinExclusiva = endDate.Date,
                    Origen = string.IsNullOrWhiteSpace(origin) ? null : origin.Trim().ToUpper(),
                    Destino = string.IsNullOrWhiteSpace(destination) ? null : destination.Trim().ToUpper()
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

            return new IncomeReportFiltersDTO
            {
                Origins = origins,
                Destinations = destinations,
                Years = years
            };
        }
    }
}
