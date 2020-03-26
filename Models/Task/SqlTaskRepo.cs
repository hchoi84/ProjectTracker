using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using ProjectTracker.Securities;

namespace ProjectTracker.Models
{
  public class SqlTaskRepo : ITask
  {
    private AppDbContext _context;
    private readonly IDataProtector _protectTaskId;

    public SqlTaskRepo(AppDbContext context, IDataProtectionProvider dataProtectionProvider, DataProtectionStrings dataProtectionStrings)
    {
      _context = context;
      _protectTaskId = dataProtectionProvider.CreateProtector(dataProtectionStrings.TaskId);
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
      return await _context.Tasks
        .Include(t => t.Member)
        .Include(t => t.TaskStatus)
        .ToListAsync();
    }

    public async Task<List<Task>> GetAllTasksOfProjectIdAsync(int projectId)
    {
      return (await _context.Tasks
        .Where(t => t.ProjectId == projectId)
        .Include(t => t.Member)
        .Include(t => t.TaskStatus)
        .OrderBy(t => t.TaskStatus.OrderPriority)
        .ToListAsync())
        .Select(t => {
          t.EncryptedId = ProtectTaskId(t.Id);
          return t;
        })
        .ToList();
    }

    public async Task<List<Task>> GetAllTasksOfProjectIdsAsync(List<int> projectIds)
    {
      return (await _context.Tasks
        .Where(t => projectIds.Contains(t.ProjectId))
        .Include(t => t.Member)
        .Include(t => t.TaskStatus)
        .OrderBy(t => t.TaskStatus.OrderPriority)
        .ToListAsync())
        .Select(t => {
          t.EncryptedId = ProtectTaskId(t.Id);
          return t;
        })
        .ToList();
    }

    public async Task<Task> GetTaskAsync(int id)
    {
      return await _context.Tasks
        .Include(t => t.Member)
        .Include(t => t.TaskStatus)
        .OrderBy(t => t.TaskStatus.OrderPriority)
        .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<List<Task>> GetTasksByMemberIds(List<string> memberIds)
    {
      return await _context.Tasks
        .Where(t => memberIds.Contains(t.MemberId))
        .Include(t => t.Member)
        .Include(t => t.TaskStatus)
          .Where(t => t.TaskStatus.StatusName != "Completed")
          .OrderBy(t => t.TaskStatus.OrderPriority)
        .ToListAsync();
    }

    public async Task<List<Task>> GetByTaskIdsAsync(List<int> taskIds)
    {
      return await _context.Tasks
        .Where(t => taskIds.Contains(t.Id))
        .Include(t => t.Member)
        .Include(t => t.TaskStatus)
        .ToListAsync();
    }

    public async Task<Task> UpdateAsync(Task updateTask)
    {
      Task task = await _context.Tasks
        .FirstOrDefaultAsync(t => t.Id == updateTask.Id);
        
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

    public string ProtectTaskId(int taskId) => _protectTaskId.Protect(taskId.ToString());

    public int UnprotectTaskId(string taskId) => Convert.ToInt32(_protectTaskId.Unprotect(taskId));
  }
}