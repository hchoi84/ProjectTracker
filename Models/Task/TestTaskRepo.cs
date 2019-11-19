using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectTracker.Models
{
  public class TestTaskRepo : ITask
  {
    private List<Task> _tasks;
    public TestTaskRepo()
    {
      _tasks = new List<Task>
      {
        new Task()
        {
          Id = 1,
          ProjectId = 1,
          StatusId = 1,
          MemberId = "1",
          TaskName = "Implement Task Feature",
          Description = "Implement CRUD operation for Project tasks",
          Created = new DateTime(2019, 11, 06, 13, 34, 00),
          Updated = new DateTime(2019, 11, 06, 13, 34, 00),
          Deadline = new DateTime(2019, 11, 06, 13, 34, 00).AddMonths(1),
        },
        new Task()
        {
          Id = 2,
          ProjectId = 1,
          StatusId = 1,
          MemberId = "1",
          TaskName = "Implement SQL",
          Description = "Transition all data to utilize SQL",
          Created = new DateTime(2019, 11, 06, 13, 34, 00),
          Updated = new DateTime(2019, 11, 06, 13, 34, 00),
          Deadline = new DateTime(2019, 11, 06, 13, 34, 00).AddMonths(1),
        },
        new Task()
        {
          Id = 3,
          ProjectId = 1,
          StatusId = 1,
          MemberId = "1",
          TaskName = "Design touch-up on Project view",
          Description = "Project title and description to go on top. Rest can stay the same for now",
          Created = new DateTime(2019, 11, 06, 13, 34, 00),
          Updated = new DateTime(2019, 11, 06, 13, 34, 00),
          Deadline = new DateTime(2019, 11, 06, 13, 34, 00).AddMonths(1),
        }
      };
    }

    public Task Add(Task task)
    {
      task.Id = _tasks.Max(t => t.Id) + 1;
      task.Created = DateTime.Now;
      task.Updated = DateTime.Now;
      _tasks.Add(task);
      return task;
    }

    public Task Delete(int id)
    {
      Task task = _tasks.FirstOrDefault(t => t.Id == id);
      if (task != null)
      {
        _tasks.Remove(task);
        return task;
      }
      return null;
    }

    public IEnumerable<Task> GetAllTasks()
    {
      return _tasks;
    }

    public Task GetTask(int id)
    {
      return _tasks.FirstOrDefault(t => t.Id == id);
    }

    public Task Update(Task updateTask)
    {
      Task task = _tasks.FirstOrDefault(t => t.Id == updateTask.Id);
      if (task != null)
      {
        task.ProjectId = updateTask.ProjectId;
        task.StatusId = updateTask.StatusId;
        task.MemberId = updateTask.MemberId;
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