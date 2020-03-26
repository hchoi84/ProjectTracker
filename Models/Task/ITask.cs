using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectTracker.Models
{
  public interface ITask
  {
    Task<Task> AddAsync(Task task);
    Task<Task> GetTaskAsync(int id);
    Task<List<Task>> GetAllTasksAsync();
    Task<List<Task>> GetAllTasksOfProjectIdAsync(int projectId);
    Task<List<Task>> GetAllTasksOfProjectIdsAsync(List<int> projectIds);
    Task<List<Task>> GetTasksByMemberIds(List<string> memberIds);
    Task<List<Task>> GetByTaskIdsAsync(List<int> taskIds);
    Task<Task> UpdateAsync(Task task);
    Task<Task> DeleteAsync(int id);
    string ProtectTaskId(int taskId);
    int UnprotectTaskId(string taskId);
  }
}