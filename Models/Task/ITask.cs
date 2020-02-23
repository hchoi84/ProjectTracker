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
    List<Task> GetAllTasksOfProjectId(int projectId);
    Task<List<Task>> GetTasksByMemberIds(List<string> memberIds);
    Task<Task> UpdateAsync(Task task);
    Task<Task> DeleteAsync(int id);
  }
}