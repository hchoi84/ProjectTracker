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

    public async Task<List<ProjectMember>> AddAsync(int projectId, List<string> projectMemberIdsToAdd)
    {
      foreach (var projectMemberIdToAdd in projectMemberIdsToAdd)
      {
        ProjectMember pm = new ProjectMember()
        {
          ProjectId = projectId,
          MemberId = projectMemberIdToAdd,
        };

        await _context.ProjectMembers.AddAsync(pm);
      }

      await _context.SaveChangesAsync();

      return await _context.ProjectMembers.Where(pm => pm.ProjectId == projectId).ToListAsync();
    }

    public async Task<List<ProjectMember>> RemoveAsync(int projectId, List<string> projectMemberIdsToRemove)
    {
      foreach (var projectMemberIdToRemove in projectMemberIdsToRemove)
      {
        ProjectMember pmToRemove = _context.ProjectMembers.FirstOrDefault(pm => pm.ProjectId == projectId && pm.MemberId == projectMemberIdToRemove);
        if (pmToRemove != null)
        {
          _context.ProjectMembers.Remove(pmToRemove);
        }
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
  
    public async Task<List<ProjectMember>> GetAllAsync()
    {
      return await _context.ProjectMembers
        .Include(pm => pm.Project)
        .ToListAsync();
    }

    public async Task<List<ProjectMember>> GetAllAsync(string memberId)
    {
      return await _context.ProjectMembers
        .Where(pm => pm.MemberId == memberId)
        .Include(pm => pm.Project)
        .ToListAsync();
    }

    public async Task<List<ProjectMember>> GetByMemberIdAsync(string memberId)
    {
      return await _context.ProjectMembers
        .Where(pm => pm.MemberId == memberId)
        .ToListAsync();
    }
  }
}