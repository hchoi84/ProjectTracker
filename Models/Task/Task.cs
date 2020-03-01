using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProjectTracker.Utilities;

namespace ProjectTracker.Models
{
  public class Task
  {
    [Key]
    public int Id { get; set; }

    [ForeignKey("Project")]
    public int ProjectId { get; set; }

    [ForeignKey("TaskStatus")]
    public int StatusId { get; set; }

    [ForeignKey("Member")]
    public string MemberId { get; set; }

    [Required(ErrorMessage="TaskName is required")]
    [Range(2, 25, ErrorMessage="Must be between 2 to 25 characters")]
    public string TaskName { get; set; }

    [Required(ErrorMessage="Description is required")]
    [Range(2, 50, ErrorMessage="Must be between 2 to 50 characters")]
    public string Description { get; set; }
    
    [Required(ErrorMessage="A project must have a deadline")]
    [DataType(DataType.Date)]
    [FutureDate(ErrorMessage="Deadline must be in the future")]
    public DateTime Deadline { get; set; }
    
    public DateTime Created { get; set; } = DateTime.Now;
    public DateTime Updated { get; set; } = DateTime.Now;

    public TaskStatus TaskStatus { get; set; }
    
    public Member Member { get; set; }
  }
}