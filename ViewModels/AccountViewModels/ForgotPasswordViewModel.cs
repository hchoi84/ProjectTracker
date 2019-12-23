using System.ComponentModel.DataAnnotations;

namespace ProjectTracker.ViewModels
{
  public class ForgotPasswordViewModel
  {
    [Required]
    [EmailAddress]
    public string Email { get; set; }
  }
}