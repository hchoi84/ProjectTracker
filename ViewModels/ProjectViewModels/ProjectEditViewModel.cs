using ProjectTracker.Models;
using System.Collections.Generic;

namespace ProjectTracker.ViewModels
{
  public class ProjectEditViewModel
  {
    public ProjectEditViewModel()
    {
      Project = new Project();
      Members = new List<Member>();
    }
    
    public Project Project { get; set; }
    public List<Member> Members { get; set; }
    public List<Member> MembersAvaileblToAdd { get; set; }
    public List<string> ProjectMemberIdsToAdd { get; set; }
    public List<Member> MembersAvaileblToRemove { get; set; }
    public List<string> ProjectMemberIdsToRemove { get; set; }
  }
}