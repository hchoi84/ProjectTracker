using System;
using System.ComponentModel.DataAnnotations;

namespace ProjectTracker.Models
{
  public class TaskStatus
  {
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage="Order Priority is required")]
    public int OrderPriority { get; set; }

    [Required(ErrorMessage="Status Name is required")]
    [Range(2, 15, ErrorMessage="Must be 2 to 15 characters long")]
    public string StatusName { get; set; }

    public bool IsDefault { get; set; }

    public DateTime Created { get; set; } = DateTime.Now;
    public DateTime Updated { get; set; } = DateTime.Now;
  }
}