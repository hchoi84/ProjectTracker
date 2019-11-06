using System.Collections.Generic;

namespace ProjectTracker.Models
{
  public interface ITask
  {
    ProjectTask Add(ProjectTask task);
    ProjectTask GetTask(int id);
    IEnumerable<ProjectTask> GetAllTasks();
    ProjectTask Update(ProjectTask task);
    ProjectTask Delete(int id);
  }
}