using AirDreams.API.Models.Dtos;
using AirDreams.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace AirDreams.API.Controllers
{
[Route("api/[controller]")]
[ApiController]
public class AirportController : ControllerBase
{
private readonly IAirportService _airportService;
    public AirportController(IAirportService airportService)
    {
        _airportService = airportService;
    }

    [HttpGet]
    public async Task<ActionResult<List<AirportDto>>> GetAll()
    {
        var airports = await _airportService.GetAllAsync();
        return Ok(airports);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AirportDto>> GetById(string id)
    {
        var airport = await _airportService.GetByCodeAsync(id);
        if (airport == null)
            return NotFound(new { message = "Airport not found" });

        return Ok(airport);
    }
}

}