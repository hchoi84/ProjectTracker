using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ProjectTracker.ViewModels;

namespace ProjectTracker.Models
{
  public interface IMember
  {
    Task<IdentityResult> RegisterAsync(RegisterViewModel newMember);
    Task<Member> GetMemberByIdAsync(string id);
    Task<List<Member>> GetAllMembersAsync();
    Task<IdentityResult> UpdateAsync(MemberEditViewModel member);
    Task<IdentityResult> DeleteAsync(string id);
    Task<IdentityResult> UpdatePassword(MemberEditViewModel editVM);
    Task<IList<Claim>> GetMemberClaimsAsync(Member member);
  }
}