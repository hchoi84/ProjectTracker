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
          IsDefault = true,
          Created = new DateTime(2019, 11, 09),
          Updated = new DateTime(2019, 11, 09)
        },
        new TaskStatus
        {
          Id = 2,
          OrderPriority = 40,
          StatusName = "Assigned",
          IsDefault = false,
          Created = new DateTime(2019, 11, 09),
          Updated = new DateTime(2019, 11, 09)
        },
        new TaskStatus
        {
          Id = 3,
          OrderPriority = 60,
          StatusName = "Completed",
          IsDefault = false,
          Created = new DateTime(2019, 11, 09),
          Updated = new DateTime(2019, 11, 09)
        }
      };
    }

    public TaskStatus Add(TaskStatus taskStatus)
    {
      taskStatus.Id = _taskStatus.Max(ts => ts.Id) + 1;
      if (taskStatus.OrderPriority == 0)
      {
        taskStatus.OrderPriority = _taskStatus.Max(ts => ts.OrderPriority) + 20;
      }

      // Singleton on IsDefault Task Status
      if (taskStatus.IsDefault)
      {
        _taskStatus.FirstOrDefault(ts => ts.IsDefault).IsDefault = false;
      }
      taskStatus.Created = DateTime.Now;
      taskStatus.Updated = DateTime.Now;
      _taskStatus.Add(taskStatus);
      return taskStatus;
    }

    public TaskStatus Delete(int id)
    {
      TaskStatus taskStatusToDelete = _taskStatus.FirstOrDefault(ts => ts.Id == id);
      if (taskStatusToDelete != null)
      {
        _taskStatus.Remove(taskStatusToDelete);
        return taskStatusToDelete;
      }
      return null;
    }

    public IEnumerable<TaskStatus> GetAllTaskStatus()
    {
      return _taskStatus;
    }

    public TaskStatus GetTaskStatus(int id)
    {
      return _taskStatus.FirstOrDefault(ts => ts.Id == id);
    }

    public TaskStatus Update(TaskStatus taskStatus)
    {
      TaskStatus taskStatusToUpdate = _taskStatus.FirstOrDefault(ts => ts.Id == taskStatus.Id);
      if (taskStatusToUpdate != null)
      {
        taskStatusToUpdate.OrderPriority = taskStatus.OrderPriority;
        taskStatusToUpdate.StatusName = taskStatus.StatusName;
        if (taskStatusToUpdate.IsDefault)
        {
          _taskStatus.FirstOrDefault(ts => ts.IsDefault).IsDefault = false;
          taskStatusToUpdate.IsDefault = taskStatus.IsDefault;
        }
        taskStatusToUpdate.Updated = DateTime.Now;
        return taskStatus;
      }
      return null;
    }

    public int GetDefaultTaskStatus()
    {
      return _taskStatus.FirstOrDefault(ts => ts.IsDefault).Id;
    }
  }
}