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
    
    [NotMapped]
    public string EncryptedId { get; set; }

    [ForeignKey("Member")]
    public string MemberId { get; set; }

    [NotMapped]
    public string EncryptedMemberId { get; set; }
    
    [Required(ErrorMessage = "Project Name is Required")]
    // [UIHint()]  <== what's this??
    [StringLength(30, MinimumLength = 2, ErrorMessage = "Value must be between {2} to {1} characters")]
    public string ProjectName { get; set; }
    
    [Required(ErrorMessage = "Summary is Required")]
    [StringLength(300, MinimumLength = 2, ErrorMessage = "Value must be between {2} to {1} characters")]
    public string Summary { get; set; }
    
    [Required(ErrorMessage = "Deadline is Required")]
    [DataType(DataType.Date)]
    [FutureDate(ErrorMessage="Deadline must be in the future")]
    public DateTime Deadline { get; set; }

    public DateTime Created { get; set; } = DateTime.Now;
    public DateTime Updated { get; set; } = DateTime.Now;
    
    public List<Task> Tasks { get; set; }
    public Member Member { get; set; }
  }
}