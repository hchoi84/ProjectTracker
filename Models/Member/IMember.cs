using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ProjectTracker.ViewModels;

namespace ProjectTracker.Models
{
  public interface IMember
  {
    Task<IdentityResult> RegisterAsync(RegisterViewModel newMember);
    Task<Member> GetMemberAsync(string id);
    Task<List<Member>> GetAllMembersAsync();
    Task<IdentityResult> UpdateAsync(RegisterViewModel member);
    Task<IdentityResult> DeleteAsync(string id);
    Task<IdentityResult> UpdatePassword(AdminEditViewModel editVM);
  }
}