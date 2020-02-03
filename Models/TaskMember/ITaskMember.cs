using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectTracker.Models
{
  public interface ITaskMember
  {
    Task<List<TaskMember>> AddMembersAsync(int taskId, List<string> taskMemberIds);
    Task<List<TaskMember>> RemoveMembersAsync(int taskId, List<string> taskMemberIds);
    Task<List<TaskMember>> GetAllMembersForTaskAsync(int taskId);
    void RemoveMemberFromTasksAsync(List<int> taskIds, string memberId);
  }
}