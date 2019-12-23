using System.ComponentModel.DataAnnotations;

namespace ProjectTracker.ViewModels
{
  public class ResetPasswordViewModel
  {
    public string Email { get; set; }
    public string Token { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Does not match with Password")]
    public string ConfirmPassword { get; set; }
  }
}