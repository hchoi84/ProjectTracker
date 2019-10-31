using System;

namespace ProjectTracker.Models
{
  public class Project
  {
    public int Id { get; set; }
    public string ProjectName { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
    public DateTime Deadline { get; set; }
    public string Creator { get; set; }
    public string Summary { get; set; }
  }
}