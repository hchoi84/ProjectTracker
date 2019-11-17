using ProjectTracker.Models;
using System.Collections.Generic;

namespace ProjectTracker.ViewModels
{
  public class ProjectViewModel
  {
    public ProjectViewModel()
    {
      Project = new Project();
      Members = new List<Member>();
    }
    
    public Project Project { get; set; }
    public List<Member> Members { get; set; }
  }
}