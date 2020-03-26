using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ProjectTracker.Models
{
  public interface IProject
  {
    Task<Project> AddAsync(Project newProject);
    Task<Project> GetProjectByIdAsync(int id);
    Task<List<Project>> GetAllProjectsAsync();
    Task<Project> UpdateAsync(Project updatedProject);
    Task<Project> DeleteAsync(int id);
    Task<bool> IsUnique(string projectName, int id = 0);
    Task<List<Project>> GetProjectsByMemberIdAsync(string memberId);
    string ProtectProjectId(int projectId);
    int UnprotectProjectId(string projectId);
  }
}