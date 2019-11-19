using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectTracker.Models
{
  public class Project
  {
    public int Id { get; set; }
    public string MemberId { get; set; }
    public string ProjectName { get; set; }
    public string Summary { get; set; }
    
    [DataType(DataType.Date)]
    public DateTime Deadline { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
    public List<Task> Tasks { get; set; }

    public Member Member { get; set; }
  }
}