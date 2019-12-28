using System.ComponentModel.DataAnnotations;

namespace ProjectTracker.ViewModels
{
  public class MemberEditViewModel
  {
    public string Id { get; set; }

    [Display(Name = "First Name")]
    public string FirstName { get; set; }

    [Display(Name = "Last Name")]
    public string LastName { get; set; }

    [Display(Name = "Email")]
    public string Email { get; set; }

    [Display(Name = "Password")]
    public string Password { get; set; }

    [Display(Name = "Old Password")]
    [DataType(DataType.Password)]
    public string OldPassword { get; set; }

    [Display(Name = "New Password")]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; }

    [Display(Name = "Confirm New Password")]
    [Compare("NewPassword", ErrorMessage = "Doesn't match with New Password")]
    [DataType(DataType.Password)]
    public string NewConfirmPassword { get; set; }
  }
}