using System.Collections.Generic;

namespace ProjectTracker.Models
{
  public interface ITaskStatus
  {
    TaskStatus Add(TaskStatus taskStatus);
    IEnumerable<TaskStatus> GetAllTaskStatus();
    TaskStatus Update(TaskStatus taskStatus);
    TaskStatus Delete(TaskStatus taskStatus);
  }
}