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
    Task<Member> GetMemberByIdAsync(string memberId);
    Task<List<Member>> GetAllMembersAsync();
    Task<IdentityResult> UpdateUserInfo(MemberEditViewModel member);
    Task<IdentityResult> UpdateAccessPermission(MemberEditViewModel memberEditVM);
    Task<IdentityResult> DeleteAsync(string memberId);
    Task<IdentityResult> UpdatePassword(MemberEditViewModel editVM);
    Task<IList<Claim>> GetMemberClaimsAsync(Member member);
    Task<string> GetMemberByEmailAsync(string memberEmail);
  }
}