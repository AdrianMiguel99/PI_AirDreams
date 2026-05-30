using System.ComponentModel.DataAnnotations;

namespace AirDreams.API.Models.Dtos
{
    public class PaymentRequestDto
    {
        [Required]
        public string TransactionId { get; set; }

        [Required]
        public string PaymentMethod { get; set; } 

        [Required, MaxLength(100)]
        public string BuyerName { get; set; }

        [Required, EmailAddress, MaxLength(100)]
        public string BuyerEmail { get; set; }

        [Phone, MaxLength(20)]
        public string? BuyerPhone { get; set; }

        [CreditCard]
        public string? CardNumber { get; set; }

        [RegularExpression(@"^(0[1-9]|1[0-2])\/([0-9]{2})$")]
        public string? CardExpiry { get; set; }

        [StringLength(4, MinimumLength = 3)]
        public string? CardCvv { get; set; }
    }
}