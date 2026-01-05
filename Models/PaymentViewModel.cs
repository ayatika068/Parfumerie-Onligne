using System.ComponentModel.DataAnnotations;

namespace ParfumerieOnline.Models
{
        public class PaymentViewModel : IValidatableObject
    {
        [Display(Name = "Mode de paiement")]
        public string PaymentMethod { get; set; } = "Card";

        [Display(Name = "Nom du titulaire")]
        public string? CardHolderName { get; set; }

        [CreditCard(ErrorMessage = "Numéro de carte invalide.")]
        [Display(Name = "Numéro de carte")]
        public string? CardNumber { get; set; }

        [RegularExpression(@"^(0[1-9]|1[0-2])\/?([0-9]{2})$", ErrorMessage = "Format invalide (MM/YY).")]
        [Display(Name = "Date d'expiration (MM/YY)")]
        public string? ExpiryDate { get; set; }

        [RegularExpression(@"^[0-9]{3,4}$", ErrorMessage = "CVV invalide (3 ou 4 chiffres).")]
        [Display(Name = "CVV")]
        public string? CVV { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (PaymentMethod == "Card")
            {
                if (string.IsNullOrWhiteSpace(CardHolderName))
                {
                    yield return new ValidationResult("Le nom du titulaire est requis.", new[] { nameof(CardHolderName) });
                }
                if (string.IsNullOrWhiteSpace(CardNumber))
                {
                    yield return new ValidationResult("Le numéro de carte est requis.", new[] { nameof(CardNumber) });
                }
                if (string.IsNullOrWhiteSpace(ExpiryDate))
                {
                    yield return new ValidationResult("La date d'expiration est requise.", new[] { nameof(ExpiryDate) });
                }
                if (string.IsNullOrWhiteSpace(CVV))
                {
                    yield return new ValidationResult("Le cryptogramme est requis.", new[] { nameof(CVV) });
                }
            }
        }
    }
}
