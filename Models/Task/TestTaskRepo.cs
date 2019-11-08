using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectTracker.Models
{
  public class TestTaskRepo : ITask
  {
    private List<ProjectTask> _tasks;
    public TestTaskRepo()
    {
      _tasks = new List<ProjectTask>
      {
        new ProjectTask()
        {
          Id = 1,
          ProjectId = 1,
          TaskName = "Implement Task Feature",
          Description = "Implement CRUD operation for Project tasks",
          Created = new DateTime(2019, 11, 06, 13, 34, 00),
          Updated = new DateTime(2019, 11, 06, 13, 34, 00),
          Deadline = new DateTime(2019, 11, 06, 13, 34, 00).AddMonths(1),
          Creator = "Howard Choi"
        },
        new ProjectTask()
        {
          Id = 2,
          ProjectId = 1,
          TaskName = "Implement SQL",
          Description = "Transition all data to utilize SQL",
          Created = new DateTime(2019, 11, 06, 13, 34, 00),
          Updated = new DateTime(2019, 11, 06, 13, 34, 00),
          Deadline = new DateTime(2019, 11, 06, 13, 34, 00).AddMonths(1),
          Creator = "Howard Choi"
        },
        new ProjectTask()
        {
          Id = 3,
          ProjectId = 1,
          TaskName = "Design touch-up on Project view",
          Description = "Project title and description to go on top. Rest can stay the same for now",
          Created = new DateTime(2019, 11, 06, 13, 34, 00),
          Updated = new DateTime(2019, 11, 06, 13, 34, 00),
          Deadline = new DateTime(2019, 11, 06, 13, 34, 00).AddMonths(1),
          Creator = "Howard Choi"
        }
      };
    }

    public ProjectTask Add(ProjectTask task)
    {
      int id = _tasks.Max(t => t.Id) + 1;
      task.Id = id;
      task.Created = DateTime.Now;
      task.Updated = DateTime.Now;
      _tasks.Add(task);
      return task;
    }

    public ProjectTask Delete(int id)
    {
      ProjectTask task = _tasks.FirstOrDefault(t => t.Id == id);
      if (task != null)
      {
        _tasks.Remove(task);
        return task;
      }
      return null;
    }

    public IEnumerable<ProjectTask> GetAllTasks()
    {
      return _tasks;
    }

    public ProjectTask GetTask(int id)
    {
      return _tasks.FirstOrDefault(t => t.Id == id);
    }

    public ProjectTask Update(ProjectTask updateTask)
    {
      ProjectTask task = _tasks.FirstOrDefault(t => t.Id == updateTask.Id);
      if (task != null)
      {
        task.ProjectId = updateTask.ProjectId;
        task.TaskName = updateTask.TaskName;
        task.Description = updateTask.Description;
        task.Deadline = updateTask.Deadline;
        task.Updated = DateTime.Now;
        return task;
      }
      return null;
    }
  }
}