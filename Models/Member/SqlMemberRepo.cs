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
    private readonly UserManager<Member> _userManager;
    private readonly IDataProtector _protectMemberId;
    public SqlMemberRepo(UserManager<Member> userManager, IDataProtectionProvider dataProtectionProvider, DataProtectionStrings dataProtectionStrings)
    {
      _userManager = userManager;
      _protectMemberId = dataProtectionProvider.CreateProtector(dataProtectionStrings.MemberId);
    }

    public async Task<IdentityResult> DeleteAsync(string memberId)
    {
      string decryptId = _protectMemberId.Unprotect(memberId);
      Member member = await _userManager.FindByIdAsync(decryptId);
      return await _userManager.DeleteAsync(member);
    }

    public async Task<List<Member>> GetAllMembersAsync()
    {
      return (await _userManager.Users.ToListAsync())
        .Select(m => {
          m.EncryptedId = _protectMemberId.Protect(m.Id);
          return m;
        })
        .OrderBy(m => m.GetFullName)
        .ToList();
    } 

    public async Task<Member> GetMemberByIdAsync(string memberId)
    {
      string decryptId = _protectMemberId.Unprotect(memberId);
      return await _userManager.FindByIdAsync(decryptId);
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

      var result = await _userManager.CreateAsync(member, newMember.Password);
      
      if (result.Succeeded)
      {
        await SetClaim(member);
      }
      
      return result;
    }

    public async Task<IdentityResult> RegisterExternalLogin(Member member, ExternalLoginInfo info)
    {
      IdentityResult result = await _userManager.CreateAsync(member);

      if (result.Succeeded)
      {
        await SetClaim(member);
      }
      else
      {
        return result;
      }

      return await _userManager.AddLoginAsync(member, info);
    }

    private async System.Threading.Tasks.Task SetClaim(Member member)
    {
      var count = _userManager.Users.Count();
      if (count <= 1)
      {
        Claim newClaim = new Claim(ClaimType.SuperAdmin.ToString(), "true");
        await _userManager.AddClaimAsync(member, newClaim);
      }
      else
      {
        Claim newClaim = new Claim(ClaimType.Member.ToString(), "true");
        await _userManager.AddClaimAsync(member, newClaim);
      }
    }

    public async Task<IdentityResult> UpdateUserInfo(MemberEditViewModel memberEditVM)
    {
      string id = _protectMemberId.Unprotect(memberEditVM.EncryptedId);
      Member member = await _userManager.FindByIdAsync(id);
      member.FirstName = memberEditVM.FirstName;
      member.LastName = memberEditVM.LastName;
      member.Email = memberEditVM.Email;

      return await _userManager.UpdateAsync(member);
    }

    public async Task<IdentityResult> UpdateAccessPermission(MemberEditViewModel memberEditVM)
    {
      IdentityResult result = IdentityResult.Success;

      string id = _protectMemberId.Unprotect(memberEditVM.EncryptedId);
      Member member = await _userManager.FindByIdAsync(id);
      List<Claim> newClaims = memberEditVM.MemberClaims
        .Select(mc => new Claim(mc.ClaimType, mc.IsSelected ? "true" : "false")).ToList();
      List<Claim> memberClaims = await _userManager.GetClaimsAsync(member) as List<Claim>;

      foreach (Claim newClaim in newClaims)
      {
        Claim memberClaim = memberClaims.FirstOrDefault(mc => mc.Type == newClaim.Type);

        if (memberClaim != null)
        {
          result = await _userManager.ReplaceClaimAsync(member, memberClaim, newClaim);
        }
        else
        {
          result = await _userManager.AddClaimAsync(member, newClaim);  
        }

        if (!result.Succeeded)
          return result;
      }

      return result;
    }

    public async Task<IdentityResult> UpdatePassword(MemberEditViewModel editVM)
    {
      string id = _protectMemberId.Unprotect(editVM.EncryptedId);
      Member member = await _userManager.FindByIdAsync(id);
      return await _userManager.ChangePasswordAsync(member, editVM.OldPassword, editVM.NewPassword);
    }

    public async Task<IList<Claim>> GetMemberClaimsAsync(Member member) => await _userManager.GetClaimsAsync(member);

    public async Task<string> GetMemberFullNameByEmailAsync(string memberEmail)
    {
      return (await _userManager.FindByEmailAsync(memberEmail)).GetFullName;
    }

    public async Task<Member> GetMemberByEmailAsync(string memberEmail)
    {
      return await _userManager.FindByEmailAsync(memberEmail);
    }

    public string ProtectMemberId(string memberId) => _protectMemberId.Protect(memberId);
    
    public string UnprotectMemberId(string memberId) => _protectMemberId.Unprotect(memberId);

    public string GetMemberId(ClaimsPrincipal member) => _userManager.GetUserId(member);
  }
}