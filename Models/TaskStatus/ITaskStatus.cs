using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectTracker.Models
{
  public interface ITaskStatus
  {
    Task<TaskStatus> AddAsync(TaskStatus taskStatus);
    Task<List<TaskStatus>> GetAllTaskStatusAsync();
    Task<TaskStatus> GetTaskStatusAsync(int id);
    Task<TaskStatus> UpdateAsync(TaskStatus taskStatus);
    Task<TaskStatus> DeleteAsync(int id);
    Task<int> GetDefaultTaskStatusAsync();
  }
}