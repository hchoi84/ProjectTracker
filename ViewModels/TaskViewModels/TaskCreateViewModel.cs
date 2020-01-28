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
      MembersAvailableToAdd = new List<Member>();
      TaskMemberIdsToAdd = new List<string>();
      MembersAvailableToRemove = new List<Member>();
      TaskMemberIdsToRemove = new List<string>();
      MembersPartOfProject = new List<Member>();
    }
    public Task Task { get; set; }
    public List<TaskStatus> TaskStatuses { get; set; }

    public List<Member> MembersAvailableToAdd { get; set; }
    public List<string> TaskMemberIdsToAdd { get; set; }
    public List<Member> MembersAvailableToRemove { get; set; }
    public List<string> TaskMemberIdsToRemove { get; set; }
    public List<Member> MembersPartOfProject { get; set; }
  }
}