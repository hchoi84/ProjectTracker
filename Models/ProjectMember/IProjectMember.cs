using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectTracker.Models
{
  public interface IProjectMember
  {
    Task<List<ProjectMember>> UpdateAsync(int projectId, List<string> projectMemberIds);
    Task<List<ProjectMember>> GetAllMembersForProjectAsync(int projectId);
  }
}