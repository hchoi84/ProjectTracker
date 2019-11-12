using System.Collections.Generic;

namespace ProjectTracker.Models
{
  public interface ITaskStatus
  {
    TaskStatus Add(TaskStatus taskStatus);
    IEnumerable<TaskStatus> GetAllTaskStatus();
    TaskStatus GetTaskStatus(int id);
    TaskStatus Update(TaskStatus taskStatus);
    TaskStatus Delete(int id);
    int GetDefaultTaskStatus();
  }
}