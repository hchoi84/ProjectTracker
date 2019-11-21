using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProjectTracker.Utilities;

namespace ProjectTracker.Models
{
  public class Project
  {
    [Key]
    public int Id { get; set; }

    [ForeignKey("Member")]
    public string MemberId { get; set; }
    
    [Required(ErrorMessage="Project Name is required")]
    [Range(2, 25, ErrorMessage="Must be between 2 to 25 characters")]
    public string ProjectName { get; set; }
    
    [Required(ErrorMessage="Summary is required")]
    [Range(2, 50, ErrorMessage="Must be between 2 to 50 characters")]
    public string Summary { get; set; }
    
    [Required(ErrorMessage="A project must have a deadline")]
    [DataType(DataType.Date)]
    [FutureDate(ErrorMessage="Deadline must be in the future")]
    public DateTime Deadline { get; set; }

    public DateTime Created { get; set; } = DateTime.Now;
    public DateTime Updated { get; set; } = DateTime.Now;
    
    public List<Task> Tasks { get; set; }
    public Member Member { get; set; }
  }
}