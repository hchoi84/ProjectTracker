using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectTracker.Models
{
  public interface ITaskMember
  {
    Task<List<TaskMember>> AddAsync(int taskId, List<string> taskMemberIdsToAdd);
    Task<List<TaskMember>> RemoveAsync(int taskId, List<string> taskMemberIdsToRemove);
    Task<List<TaskMember>> GetAllMembersForTaskAsync(int taskId);
  }
}