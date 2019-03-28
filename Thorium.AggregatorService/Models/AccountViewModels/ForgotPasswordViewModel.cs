using System.ComponentModel.DataAnnotations;

namespace Thorium.Aggregator.Models.AccountViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
