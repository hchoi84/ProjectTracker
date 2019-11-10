using System.Collections.Generic;

namespace ProjectTracker.Models
{
  public interface ITask
  {
    Task Add(Task task);
    Task GetTask(int id);
    IEnumerable<Task> GetAllTasks();
    Task Update(Task task);
    Task Delete(int id);
  }
}