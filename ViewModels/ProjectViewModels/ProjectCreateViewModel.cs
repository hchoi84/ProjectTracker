using System;
using System.ComponentModel.DataAnnotations;
using ProjectTracker.Utilities;

namespace ProjectTracker.ViewModels
{
  public class ProjectCreateViewModel
  {
    [Required(ErrorMessage = "(Required)")]
    [StringLength(30, MinimumLength = 2, ErrorMessage = "Value must be between {2} to {1} characters")]
    public string ProjectName { get; set; }
    
    [Required(ErrorMessage = "(Required)")]
    [StringLength(300, MinimumLength = 2, ErrorMessage = "Value must be between {2} to {1} characters")]
    public string Summary { get; set; }
    
    [Required(ErrorMessage = "(Required)")]
    [DataType(DataType.Date)]
    [FutureDate(ErrorMessage="Deadline must be in the future")]
    public DateTime Deadline { get; set; }
  }
}