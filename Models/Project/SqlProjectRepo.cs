using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

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
        await _context.SaveChangesAsync();
        return project;
      }
      return null;
    }

    public async Task<List<Project>> GetAllProjectsAsync()
    {
      var projects = await _context.Projects.Include(p => p.Member).ToListAsync();

      return projects;
    }

    public async Task<Project> GetProjectAsync(int id)
    {
      Project project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == id);
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

        await _context.SaveChangesAsync();
        return project;
      }
      return null;
    }
  }
}