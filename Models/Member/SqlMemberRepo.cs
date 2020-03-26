using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectTracker.Securities;
using ProjectTracker.Utilities;
using ProjectTracker.ViewModels;

namespace ProjectTracker.Models
{
  public class SqlMemberRepo : IMember
  {
    private readonly UserManager<Member> _member;
    private readonly IDataProtector _protector;
    public SqlMemberRepo(UserManager<Member> member, IDataProtectionProvider dataProtectionProvider, DataProtectionStrings dataProtectionStrings)
    {
      _member = member;
      _protector = dataProtectionProvider.CreateProtector(dataProtectionStrings.MemberId);
    }

    public async Task<IdentityResult> DeleteAsync(string id)
    {
      Member member = await _member.FindByIdAsync(id);
      return await _member.DeleteAsync(member);
    }

    public async Task<List<Member>> GetAllMembersAsync()
    {
      return (await _member.Users.ToListAsync())
        .OrderBy(m => m.GetFullName)
        .ToList();
    } 

    public async Task<Member> GetMemberByIdAsync(string id) => await _member.FindByIdAsync(id);

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

      var result = await _member.CreateAsync(member, newMember.Password);
      
      if (result.Succeeded)
      {
        if ((await GetAllMembersAsync()).Count <= 1)
        {
          Claim newClaim = new Claim(ClaimType.SuperAdmin.ToString(), "true");
          await _member.AddClaimAsync(member, newClaim);
        }
        else
        {
          Claim newClaim = new Claim(ClaimType.Member.ToString(), "true");
          await _member.AddClaimAsync(member, newClaim);
        }
      }
      
      return result;
    }

    public async Task<IdentityResult> UpdateUserInfo(MemberEditViewModel memberEditVM)
    {
      string id = _protector.Unprotect(memberEditVM.EncryptedId);
      Member member = await _member.FindByIdAsync(id);
      member.FirstName = memberEditVM.FirstName;
      member.LastName = memberEditVM.LastName;
      member.Email = memberEditVM.Email;

      return await _member.UpdateAsync(member);
    }

    public async Task<IdentityResult> UpdateAccessPermission(MemberEditViewModel memberEditVM)
    {
      IdentityResult result = IdentityResult.Success;

      string id = _protector.Unprotect(memberEditVM.EncryptedId);
      Member member = await _member.FindByIdAsync(id);
      List<Claim> newClaims = memberEditVM.MemberClaims
        .Select(mc => new Claim(mc.ClaimType, mc.IsSelected ? "true" : "false")).ToList();
      List<Claim> memberClaims = await _member.GetClaimsAsync(member) as List<Claim>;

      foreach (Claim newClaim in newClaims)
      {
        Claim memberClaim = memberClaims.FirstOrDefault(mc => mc.Type == newClaim.Type);

        if (memberClaim != null)
        {
          result = await _member.ReplaceClaimAsync(member, memberClaim, newClaim);
        }
        else
        {
          result = await _member.AddClaimAsync(member, newClaim);  
        }

        if (!result.Succeeded)
          return result;
      }

      return result;
    }

    public async Task<IdentityResult> UpdatePassword(MemberEditViewModel editVM)
    {
      string id = _protector.Unprotect(editVM.EncryptedId);
      Member member = await _member.FindByIdAsync(id);
      return await _member.ChangePasswordAsync(member, editVM.OldPassword, editVM.NewPassword);
    }

    public async Task<IList<Claim>> GetMemberClaimsAsync(Member member) => await _member.GetClaimsAsync(member);
  }
}