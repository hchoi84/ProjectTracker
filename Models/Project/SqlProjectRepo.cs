using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ProjectTracker.Utilities;

namespace ProjectTracker.Models
{
  public class SqlProjectRepo : IProject
  {
    private readonly AppDbContext _context;
    public SqlProjectRepo(AppDbContext context)
    {
      _context = context;
    }

    public async Task<Project> AddAsync(Project newProject)
    {
      await _context.Projects.AddAsync(newProject);
      await _context.SaveChangesAsync();
      return newProject;
    }

    public async Task<Project> DeleteAsync(int id)
    {
      Project project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == id);
      if (project != null)
      {
        _context.Projects.Remove(project);

        try
        {
          await _context.SaveChangesAsync();
          return project;   
        }
        catch (DbUpdateException e)
        {
          // TODO log the error message. Where? How?
          return null;
        }
        
      }
      return null;
    }

    public async Task<List<Project>> GetAllProjectsAsync()
    {
      var projects = await _context.Projects.Include(p => p.Member).ToListAsync();

      return projects;
    }

    public async Task<Project> GetProjectByIdAsync(int id)
    {
      Project project = await _context.Projects
        .Include(p => p.Member)
        .FirstOrDefaultAsync(p => p.Id == id);
      if (project != null)
      {
        return project;
      }
      return null;
    }

    public async Task<Project> UpdateAsync(Project updatedProject)
    {
      Project project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == updatedProject.Id);
      if (project != null)
      {
        project.MemberId = updatedProject.MemberId;
        project.ProjectName = updatedProject.ProjectName;
        project.Updated = DateTime.Now;
        project.Deadline = updatedProject.Deadline;
        project.Summary = updatedProject.Summary;

        await _context.SaveChangesAsync();
        return project;
      }
      return null;
    }

    public async Task<bool> IsUnique(string projectName, int id = 0)
    {
      if (!await _context.Projects.AnyAsync())
      {
        return true;
      }

      Project project = await _context.Projects
        .Where(p => p.Id != id)
        .FirstOrDefaultAsync(p => p.ProjectName == projectName);
      return project == null ? true : false;
    }

    public async Task<List<Project>> GetProjectsByMemberId(string memberId)
    {
      return await _context.Projects
        .Where(p => p.MemberId == memberId)
        .Include(p => p.Member)
        .ToListAsync();
    }
  }
}