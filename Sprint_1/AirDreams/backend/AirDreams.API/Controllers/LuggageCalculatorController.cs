using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Dapper;                         
using System.Data;
using AirDreams.API.Models.Dtos;

namespace AirDreams.API.Controllers
{
    [ApiController]
    [Route("api/luggage/calculate")]
    public class LuggageCalculatorController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public LuggageCalculatorController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("total")]
        public async Task<IActionResult> CalculateTotal([FromBody] LuggageTotalRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            decimal totalChecked = 0, totalCarryOn = 0;
            using var connection = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            foreach (var seg in request.Segments)
            {
                if (request.CheckedQuantity > 0)
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("base", seg.CheckedPrice);
                    parameters.Add("multiplier", seg.Multiplier);
                    parameters.Add("quantity", request.CheckedQuantity);

                    totalChecked += await connection.ExecuteScalarAsync<decimal>(
                        "SELECT dbo.fn_TotalLuggageCost(@base, @multiplier, @quantity)", parameters);
                }

                if (request.CarryOnQuantity > 0)
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("base", seg.CarryOnPrice);
                    parameters.Add("multiplier", seg.Multiplier);
                    parameters.Add("quantity", request.CarryOnQuantity);

                    totalCarryOn += await connection.ExecuteScalarAsync<decimal>(
                        "SELECT dbo.fn_TotalLuggageCost(@base, @multiplier, @quantity)", parameters);
                }
            }

            return Ok(new { checkedTotal = totalChecked, carryOnTotal = totalCarryOn });
        }
    }
}