using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectTracker.ViewModels;

namespace ProjectTracker.Models
{
  public class SqlMemberRepo : IMember
  {
    private readonly UserManager<Member> _member;
    public SqlMemberRepo(UserManager<Member> member)
    {
      _member = member;
    }

    public Task<Member> DeleteAsync(int id)
    {
      throw new System.NotImplementedException();
    }

    public async Task<List<Member>> GetAllMembersAsync()
    {
      return await _member.Users.ToListAsync();
    }

    public async Task<Member> GetMemberAsync(string id)
    {
      return await _member.FindByIdAsync(id);
    }

    public async Task<IdentityResult> RegisterAsync(AdminRegisterViewModel newMember)
    {
      Member member = new Member
      {
        FirstName = newMember.FirstName,
        LastName = newMember.LastName,
        Email = newMember.Email,
        UserName = newMember.Email,
        Created = DateTime.Now,
        Updated = DateTime.Now,
      };

      return await _member.CreateAsync(member, newMember.Password);
    }

    public Task<Member> UpdateAsync(Member member)
    {
      throw new System.NotImplementedException();
    }
  }
}