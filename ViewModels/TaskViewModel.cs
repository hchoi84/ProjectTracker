using System.Collections.Generic;
using ProjectTracker.Models;

namespace ProjectTracker.ViewModels
{
  public class TaskViewModel
  {
    public Task Task { get; set; }
    public List<TaskStatus> TaskStatus { get; set; }
  }
}