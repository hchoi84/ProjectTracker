using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectTracker.Models
{
  public interface ITask
  {
    Task<Task> AddAsync(Task task);
    Task<Task> GetTaskAsync(int id);
    Task<List<Task>> GetAllTasksAsync();
    Task<Task> UpdateAsync(Task task);
    Task<Task> DeleteAsync(int id);
  }
}