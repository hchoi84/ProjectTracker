using System.Collections.Generic;
using ProjectTracker.Models;

namespace ProjectTracker.ViewModels
{
  public class TaskViewModel
  {
    public TaskViewModel()
    {
      Project = new Project();
      Tasks = new List<Task>();
      TaskStatuses = new List<TaskStatus>();
    }

    public Project Project { get; set; }
    public List<Task> Tasks { get; set; }
    public List<TaskStatus> TaskStatuses { get; set; }
  }
}