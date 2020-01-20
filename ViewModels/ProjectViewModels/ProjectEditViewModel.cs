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
      MembersAvailableToAdd = new List<Member>();
      ProjectMemberIdsToAdd = new List<string>();
      MembersAvailableToRemove = new List<Member>();
      ProjectMemberIdsToRemove = new List<string>();
    }
    
    public Project Project { get; set; }
    public List<Member> Members { get; set; }
    public List<Member> MembersAvailableToAdd { get; set; }
    public List<string> ProjectMemberIdsToAdd { get; set; }
    public List<Member> MembersAvailableToRemove { get; set; }
    public List<string> ProjectMemberIdsToRemove { get; set; }
  }
}