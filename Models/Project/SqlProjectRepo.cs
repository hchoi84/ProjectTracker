using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ProjectTracker.Securities;
using ProjectTracker.Utilities;

namespace ProjectTracker.Models
{
  public class SqlProjectRepo : IProject
  {
    private readonly AppDbContext _context;
    private readonly IDataProtector _protectProjectId;

    public SqlProjectRepo(AppDbContext context, IDataProtectionProvider dataProtectionProvider, DataProtectionStrings dataProtectionStrings)
    {
      _context = context;
      _protectProjectId = dataProtectionProvider.CreateProtector(dataProtectionStrings.ProjectId);
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
      var projects = (await _context.Projects
        .Include(p => p.Member)
        .ToListAsync())
        .Select(p => 
        {
          p.EncryptedId = _protectProjectId.Protect(p.Id.ToString());
          return p;
        })
        .ToList();

      return projects;
    }

    public async Task<Project> GetProjectByIdAsync(int id)
    {
      Project project = await _context.Projects
        .Include(p => p.Member)
        .FirstOrDefaultAsync(p => p.Id == id);
      
      if (project != null)
      {
        project.EncryptedId = _protectProjectId.Protect(project.Id.ToString());
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

    public async Task<List<Project>> GetProjectsByMemberIdAsync(string memberId)
    {
      return (await _context.Projects
        .Where(p => p.MemberId == memberId)
        .Include(p => p.Member)
        .ToListAsync())
        .Select(p => {
          p.EncryptedId = _protectProjectId.Protect(p.Id.ToString());
          return p;
        })
        .ToList();
    }

    public string ProtectProjectId(int projectId) => _protectProjectId.Protect(projectId.ToString());

    public int UnprotectProjectId(string projectId) => Convert.ToInt32(_protectProjectId.Unprotect(projectId));
  }
}