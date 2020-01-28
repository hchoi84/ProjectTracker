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

    public async Task<List<TaskMember>> AddAsync(int taskId, List<string> taskMemberIdsToAdd)
    {
      foreach (var taskMemberIdToAdd in taskMemberIdsToAdd)
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

    public async Task<List<TaskMember>> RemoveAsync(int taskId, List<string> taskMemberIdsToRemove)
    {
      foreach (var taskMemberIdToRemove in taskMemberIdsToRemove)
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
  }
}