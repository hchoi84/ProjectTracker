using System.Collections.Generic;
using ProjectTracker.Models;

namespace ProjectTracker.ViewModels
{
  public class TaskViewModel
  {
    public TaskViewModel()
    {
      Projects = new List<Project>();
      Tasks = new List<Task>();
      TaskStatus = new List<TaskStatus>();
      Members = new List<Member>();
    }

    public List<Project> Projects { get; set; }
    public List<Task> Tasks { get; set; }
    public List<TaskStatus> TaskStatus { get; set; }
    public List<Member> Members { get; set; }
  }
}