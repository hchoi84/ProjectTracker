using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ProjectTracker.ViewModels;

namespace ProjectTracker.Models
{
  public interface IMember
  {
    Task<IdentityResult> RegisterAsync(AdminRegisterViewModel newMember);
    Task<Member> GetMemberAsync(string id);
    Task<List<Member>> GetAllMembersAsync();
    Task<Member> UpdateAsync(Member member);
    Task<Member> DeleteAsync(int id);
  }
}