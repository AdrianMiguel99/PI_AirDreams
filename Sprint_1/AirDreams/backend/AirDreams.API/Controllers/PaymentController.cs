using Microsoft.AspNetCore.Mvc;
using AirDreams.API.Models.Dtos;
using AirDreams.API.Services;

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
    }
}