using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ProjectTracker.Models
{
  public class SqlTaskRepo : ITask
  {
    private AppDbContext _context;
    public SqlTaskRepo(AppDbContext context)
    {
      _context = context;
    }

    public async Task<Task> AddAsync(Task task)
    {
      task.Created = DateTime.Now;
      task.Updated = DateTime.Now;
      await _context.Tasks.AddAsync(task);
      await _context.SaveChangesAsync();
      return task;
    }

    public async Task<Task> DeleteAsync(int id)
    {
      Task task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id);
      if (task != null)
      {
        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();
        return task;
      }
      return null;
    }

    public async Task<List<Task>> GetAllTasksAsync()
    {
      return await _context.Tasks.
        Include(t => t.Project).
        Include(t => t.Member).
        Include(t => t.TaskStatus).
        ToListAsync();
    }

    public async Task<List<Task>> GetAllTasksOfProjectIdAsync(int id)
    {
      return await _context.Tasks.
        Where(t => t.ProjectId == id).
        Include(t => t.Project).
        Include(t => t.Member).
        Include(t => t.TaskStatus).
        OrderBy(t => t.TaskStatus.OrderPriority).
        ToListAsync();
    }

    public async Task<Task> GetTaskAsync(int id)
    {
      return await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<List<Task>> GetTasksByMemberIds(List<string> memberIds)
    {
      return await _context.Tasks.Where(t => memberIds.Contains(t.MemberId))
        .Include(t => t.Project)
        .Include(t => t.Member)
        .Include(t => t.TaskStatus)
          .Where(t => t.TaskStatus.StatusName != "Completed")
          .OrderBy(t => t.TaskStatus.OrderPriority)
        .ToListAsync();
    }

    public async Task<Task> UpdateAsync(Task updateTask)
    {
      Task task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == updateTask.Id);
      if (task != null)
      {
        task.StatusId = updateTask.StatusId;
        task.TaskName = updateTask.TaskName;
        task.Description = updateTask.Description;
        task.Deadline = updateTask.Deadline;
        task.Updated = DateTime.Now;

        await _context.SaveChangesAsync();
        return task;
      }
      return null;
    }

  }
}