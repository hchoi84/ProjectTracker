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
      TaskStatus = new List<TaskStatus>();
      Users = new List<User>();
    }

    public Project Project { get; set; }
    public List<Task> Tasks { get; set; }
    public List<TaskStatus> TaskStatus { get; set; }
    public List<User> Users { get; set; }
  }
}