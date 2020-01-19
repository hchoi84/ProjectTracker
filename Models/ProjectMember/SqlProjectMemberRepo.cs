using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ProjectTracker.Models
{
  public class SqlProjectMemberRepo : IProjectMember
  {
    private readonly AppDbContext _context;
    public SqlProjectMemberRepo(AppDbContext context)
    {
      _context = context;
    }

    public async Task<List<ProjectMember>> UpdateAsync(int projectId, List<string> projectMemberIds)
    {
      var projectMembers = await _context.ProjectMembers.Where(pm => pm.ProjectId == projectId).ToListAsync();
      foreach (var projectMember in projectMembers)
      {
        if (projectMemberIds.Contains(projectMember.MemberId))
        {
          projectMemberIds.Remove(projectMember.MemberId);
        }
        else
        {
          _context.ProjectMembers.Remove(projectMember);
        }
      }

      foreach (var projectMemberId in projectMemberIds)
      {
        ProjectMember pm = new ProjectMember()
        {
          ProjectId = projectId,
          MemberId = projectMemberId,
        };
      }

      await _context.SaveChangesAsync();

      return await _context.ProjectMembers.Where(pm => pm.ProjectId == projectId).ToListAsync();
    }
  
    public async Task<List<ProjectMember>> GetAllMembersForProjectAsync(int projectId)
    {
      return await _context.ProjectMembers.Where(pm => pm.ProjectId == projectId)
        .Include(pm => pm.Member)
        .ToListAsync();
    }
  }
}