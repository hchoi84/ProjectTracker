using System.Collections.Generic;
using ProjectTracker.Models;

namespace ProjectTracker.ViewModels
{
  public class TaskCreateViewModel
  {
    public TaskCreateViewModel()
    {
      Task = new Task();
      TaskStatuses = new List<TaskStatus>();
    }
    public Task Task { get; set; }
    public List<TaskStatus> TaskStatuses { get; set; }
  }
}