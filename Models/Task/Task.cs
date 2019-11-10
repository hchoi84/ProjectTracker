using System;
using System.ComponentModel.DataAnnotations;

namespace ProjectTracker.Models
{
  public class Task
  {
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public int StatusId { get; set; }
    public string TaskName { get; set; }
    public string Creator { get; set; }
    public string Description { get; set; }
    
    [DataType(DataType.Date)]
    public DateTime Deadline { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }

    public TaskStatus TaskStatus { get; set; }
  }
}