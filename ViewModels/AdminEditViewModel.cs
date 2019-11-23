using System.ComponentModel.DataAnnotations;

namespace ProjectTracker.ViewModels
{
  public class AdminEditViewModel
  {
    public string Id { get; set; }

    [Display(Name = "First Name")]
    public string FirstName { get; set; }

    [Display(Name = "Last Name")]
    public string LastName { get; set; }

    [Display(Name = "Email")]
    public string Email { get; set; }

    public bool IsPswdNull { get; set; }

    [Display(Name = "Password")]
    public string Password { get; set; }

    [Display(Name = "Old Password")]
    public string OldPassword { get; set; }

    [Display(Name = "New Password")]
    public string NewPassword { get; set; }

    [Display(Name = "Confirm New Password")]
    public string NewConfirmPassword { get; set; }
  }
}