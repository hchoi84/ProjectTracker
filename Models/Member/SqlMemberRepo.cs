using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
      IdentityResult result;

      Member member = await _member.FindByIdAsync(model.Id);
      member.FirstName = model.FirstName;
      member.LastName = model.LastName;
      member.Email = model.Email;
      result = await _member.UpdateAsync(member);

      if (!result.Succeeded)
      {
        return result;
      }

      List<Claim> newClaims = model.MemberClaims
        .Select(mc => new Claim(mc.ClaimType, mc.IsSelected ? "true" : "false")).ToList();
      List<Claim> memberClaims = await _member.GetClaimsAsync(member) as List<Claim>;
      foreach (Claim newClaim in newClaims)
      {
        Claim memberClaim = memberClaims.FirstOrDefault(mc => mc.Type == newClaim.Type);

        if (memberClaim != null)
        {
          Claim currentClaim = memberClaims.First(mc => mc.Type == newClaim.Type);
          result = await _member.ReplaceClaimAsync(member, currentClaim, newClaim);
        }
        else
        {
          result = await _member.AddClaimAsync(member, newClaim);  
        }
      }
      
      return result;
    }

    public async Task<IdentityResult> UpdatePassword(MemberEditViewModel editVM)
    {
      Member member = await _member.FindByIdAsync(editVM.Id);
      return await _member.ChangePasswordAsync(member, editVM.OldPassword, editVM.NewPassword);
    }

    public async Task<IList<Claim>> GetMemberClaimsAsync(Member member)
    {
      return await _member.GetClaimsAsync(member);
    }
  }
}