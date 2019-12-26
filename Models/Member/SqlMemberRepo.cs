using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

    public async Task<IdentityResult> DeleteAsync(string id)
    {
      Member member = await _member.FindByIdAsync(id);
      return await _member.DeleteAsync(member);
    }

    public async Task<List<Member>> GetAllMembersAsync()
    {
      return await _member.Users.ToListAsync();
    }

    public async Task<Member> GetMemberByIdAsync(string id)
    {
      return await _member.FindByIdAsync(id);
    }

    public async Task<IdentityResult> RegisterAsync(RegisterViewModel newMember)
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

    public async Task<IdentityResult> UpdateAsync(MemberEditViewModel model)
    {
      Member member = await _member.FindByIdAsync(model.Id);
      member.FirstName = model.FirstName;
      member.LastName = model.LastName;
      member.Email = model.Email;
      
      return await _member.UpdateAsync(member);
    }

    public async Task<IdentityResult> UpdatePassword(MemberEditViewModel editVM)
    {
      Member member = await _member.FindByIdAsync(editVM.Id);
      return await _member.ChangePasswordAsync(member, editVM.OldPassword, editVM.NewPassword);
    }
  }
}