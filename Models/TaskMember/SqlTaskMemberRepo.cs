using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ProjectTracker.Models
{
  public class SqlTaskMemberRepo : ITaskMember
  {
    private readonly AppDbContext _context;
    public SqlTaskMemberRepo(AppDbContext context)
    {
      _context = context;
    }

    public async Task<List<TaskMember>> AddMembersAsync(int taskId, List<string> taskMemberIds)
    {
      foreach (var taskMemberIdToAdd in taskMemberIds)
      {
        TaskMember tm = new TaskMember()
        {
          TaskId = taskId,
          MemberId = taskMemberIdToAdd,
        };

        await _context.TaskMembers.AddAsync(tm);
      }

      await _context.SaveChangesAsync();

      return await _context.TaskMembers.Where(tm => tm.TaskId == taskId).ToListAsync();
    }

    public async Task<List<TaskMember>> RemoveMembersAsync(int taskId, List<string> taskMemberIds)
    {
      foreach (var taskMemberIdToRemove in taskMemberIds)
      {
        TaskMember tmToRemove = _context.TaskMembers.FirstOrDefault(tm => tm.TaskId == taskId && tm.MemberId == taskMemberIdToRemove);
        if (tmToRemove != null)
        {
          _context.TaskMembers.Remove(tmToRemove);
        }
      }

      await _context.SaveChangesAsync();

      return await _context.TaskMembers.Where(tm => tm.TaskId == taskId).ToListAsync();
    }

    public async Task<List<TaskMember>> GetAllMembersForTaskAsync(int taskId)
    {
      return await _context.TaskMembers.Where(tm => tm.TaskId == taskId)
        .Include(tm => tm.Member)
        .ToListAsync();
    }

    public async void RemoveMemberFromTasksAsync(List<int> taskIds, string memberId)
    {
      foreach (int taskId in taskIds)
      {
        TaskMember taskMember = await _context.TaskMembers
          .DefaultIfEmpty(null)
          .FirstOrDefaultAsync(tm => tm.TaskId == taskId && tm.MemberId == memberId);

        if (taskMember != null)
        {
          _context.TaskMembers.Remove(taskMember);
        }
      }
      
      _context.SaveChanges();
    }
  }
}