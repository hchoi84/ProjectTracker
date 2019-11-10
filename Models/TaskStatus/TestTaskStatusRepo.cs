using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectTracker.Models
{
  public class TestTaskStatusRepo : ITaskStatus
  {
    private List<TaskStatus> _taskStatus;

    public TestTaskStatusRepo()
    {
      _taskStatus = new List<TaskStatus>()
      {
        new TaskStatus
        {
          Id = 1,
          OrderPriority = 20,
          StatusName = "Pending",
          Created = new DateTime(2019, 11, 09),
          Updated = new DateTime(2019, 11, 09)
        },
        new TaskStatus
        {
          Id = 2,
          OrderPriority = 40,
          StatusName = "Assigned",
          Created = new DateTime(2019, 11, 09),
          Updated = new DateTime(2019, 11, 09)
        },
        new TaskStatus
        {
          Id = 3,
          OrderPriority = 60,
          StatusName = "Completed",
          Created = new DateTime(2019, 11, 09),
          Updated = new DateTime(2019, 11, 09)
        }
      };
    }

    public TaskStatus Add(TaskStatus taskStatus)
    {
      taskStatus.Id = _taskStatus.Max(ts => ts.Id) + 1;
      if(taskStatus.OrderPriority == 0)
      {
        taskStatus.OrderPriority = _taskStatus.Max(ts => ts.OrderPriority) + 20;
      }
      taskStatus.Created = DateTime.Now;
      taskStatus.Updated = DateTime.Now;
      _taskStatus.Add(taskStatus);
      return taskStatus;
    }

    public TaskStatus Delete(TaskStatus taskStatus)
    {
      TaskStatus taskStatusToDelete = _taskStatus.FirstOrDefault(ts => ts.Id == taskStatus.Id);
      if (taskStatusToDelete != null)
      {
        _taskStatus.Remove(taskStatusToDelete);
        return taskStatus;
      }
      return null;
    }

    public IEnumerable<TaskStatus> GetAllTaskStatus() => _taskStatus;

    public TaskStatus Update(TaskStatus taskStatus)
    {
      TaskStatus taskStatusToUpdate = _taskStatus.FirstOrDefault(ts => ts.Id == taskStatus.Id);
      if (taskStatusToUpdate != null)
      {
        taskStatusToUpdate.OrderPriority = taskStatus.OrderPriority;
        taskStatusToUpdate.StatusName = taskStatus.StatusName;
        taskStatusToUpdate.Updated = DateTime.Now;
        return taskStatus;
      }
      return null;
    }
  }
}