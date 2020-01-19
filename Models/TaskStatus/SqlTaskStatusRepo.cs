using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ProjectTracker.Models
{
  public class SqlTaskStatusRepo : ITaskStatus
  {
    private AppDbContext _context;

    public SqlTaskStatusRepo(AppDbContext context)
    {
      _context = context;
    }

    public async Task<TaskStatus> AddAsync(TaskStatus taskStatus)
    {
      if (taskStatus.OrderPriority == 0)
      {
        taskStatus.OrderPriority = await _context.TaskStatuses.MaxAsync(ts => ts.OrderPriority) + 20;
      }

      // Singleton on IsDefault Task Status
      if (taskStatus.IsDefault)
      {
        TaskStatus tempTaskStatus = await _context.TaskStatuses.FirstOrDefaultAsync(ts => ts.IsDefault);
        if (tempTaskStatus != null)
        {
          tempTaskStatus.IsDefault = false;
        }
      }
      await _context.AddAsync(taskStatus);
      await _context.SaveChangesAsync();
      return taskStatus;
    }

    public async Task<TaskStatus> DeleteAsync(int id)
    {
      TaskStatus taskStatusToDelete = await _context.TaskStatuses.FirstOrDefaultAsync(ts => ts.Id == id);
      if (taskStatusToDelete != null)
      {
        _context.Remove(taskStatusToDelete);
        await _context.SaveChangesAsync();
        return taskStatusToDelete;
      }
      return null;
    }

    public async Task<List<TaskStatus>> GetAllTaskStatusAsync()
    {
      List<TaskStatus> taskStatuses = await _context.TaskStatuses.OrderBy(ts => ts.OrderPriority).ToListAsync();
      return taskStatuses;
    }

    public async Task<TaskStatus> GetTaskStatusAsync(int id)
    {
      return await _context.TaskStatuses.FirstOrDefaultAsync(ts => ts.Id == id);
    }

    public async Task<TaskStatus> UpdateAsync(TaskStatus taskStatus)
    {
      TaskStatus taskStatusToUpdate = await _context.TaskStatuses.FirstOrDefaultAsync(ts => ts.Id == taskStatus.Id);
      if (taskStatusToUpdate != null)
      {
        taskStatusToUpdate.OrderPriority = taskStatus.OrderPriority;
        taskStatusToUpdate.StatusName = taskStatus.StatusName;
        if (taskStatusToUpdate.IsDefault)
        {
          TaskStatus tempTaskStatus = await _context.TaskStatuses.FirstOrDefaultAsync(ts => ts.IsDefault);
          tempTaskStatus.IsDefault = false;
          taskStatusToUpdate.IsDefault = taskStatus.IsDefault;
        }
        taskStatusToUpdate.Updated = DateTime.Now;
        await _context.SaveChangesAsync();
        return taskStatus;
      }
      return null;
    }

    public async Task<int> GetDefaultTaskStatusAsync()
    {
      TaskStatus taskStatus = await _context.TaskStatuses.FirstOrDefaultAsync(ts => ts.IsDefault);
      return taskStatus.Id;
    }
  }
}