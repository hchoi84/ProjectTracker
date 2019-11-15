using ProjectTracker.Models;
using System.Collections.Generic;

namespace ProjectTracker.ViewModels
{
  public class ProjectViewModel
  {
    public ProjectViewModel()
    {
      project = new Project();
      users = new List<User>();
    }
    
    public Project project { get; set; }
    public List<User> users { get; set; }
  }
}