using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectTracker.Models
{
  public interface IProjectMember
  {
    Task<List<ProjectMember>> AddAsync(int projectId, List<string> projectMemberIdsToAdd);
    Task<List<ProjectMember>> RemoveAsync(int projectId, List<string> projectMemberIdsToRemove);
    Task<List<ProjectMember>> GetAllMembersForProjectAsync(int projectId);
    Task<List<ProjectMember>> GetAllAsync();
    Task<List<ProjectMember>> GetAllAsync(string memberId);
    Task<List<ProjectMember>> GetByMemberIdAsync(string memberId);
  }
}