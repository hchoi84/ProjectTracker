using System.ComponentModel.DataAnnotations;

namespace ProjectTracker.ViewModels
{
  public class AdminRegisterViewModel
  {
    public string Id { get; set; }

    [Required(ErrorMessage = "(Required)")]
    [Display(Name = "First Name")]
    [MinLength(2, ErrorMessage = "Must be between at least {1} characters")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "(Required)")]
    [Display(Name = "Last Name")]
    [MinLength(2, ErrorMessage = "Must be between at least {1} characters")]    
    public string LastName { get; set; }

    [Required(ErrorMessage = "(Required)")]
    [Display(Name = "Email")]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }

    [Required(ErrorMessage = "(Required)")]
    [Display(Name = "Password")]
    [DataType(DataType.Password)]
    [StringLength(20, MinimumLength = 10, ErrorMessage = "Must be between {2} to {1} characters")]
    public string Password { get; set; }

    [Required(ErrorMessage = "(Required)")]
    [Display(Name = "Confirm Password")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Does not match with Password")]
    public string ConfirmPassword { get; set; }
  }
}