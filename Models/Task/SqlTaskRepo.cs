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
      _context.Tasks.Add(task);
      await _context.SaveChangesAsync();
      return task;
    }

    public async Task<Task> DeleteAsync(int id)
    {
      Task task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id);
      if (task != null)
      {
        _context.Tasks.Remove(task);
        return task;
      }
      return null;
    }

    public async Task<List<Task>> GetAllTasksAsync()
    {
      return await _context.Tasks.ToListAsync();
    }

    public async Task<Task> GetTaskAsync(int id)
    {
      return await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<Task> UpdateAsync(Task updateTask)
    {
      Task task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == updateTask.Id);
      if (task != null)
      {
        task.ProjectId = updateTask.ProjectId;
        task.StatusId = updateTask.StatusId;
        task.MemberId = updateTask.MemberId;
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