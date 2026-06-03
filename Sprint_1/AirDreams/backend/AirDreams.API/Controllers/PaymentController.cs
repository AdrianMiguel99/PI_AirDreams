using AirDreams.API.DTOs;
using AirDreams.API.Models.Dtos;
using AirDreams.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace AirDreams.API.Controllers
{
    [ApiController]
    [Route("api/payment")]
    public class PaymentController : ControllerBase
    {
        private readonly IPurchaseService _purchaseService;

        public PaymentController(IPurchaseService purchaseService)
        {
            _purchaseService = purchaseService;
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPayment([FromBody] ConfirmPurchaseDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _purchaseService.ConfirmPurchaseAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("check-availability")]
        public async Task<IActionResult> CheckAvailability([FromBody] CheckFlightAvailabilityDto dto)
        {
            var available = await _purchaseService.CheckFlightAvailabilityAsync(
                dto.NumberFlight,
                dto.SeatClass,
                dto.RequestedSeats);

            return Ok(new
            {
                isAvailable = available
            });
        }

    }
}