using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectTracker.Models;
using ProjectTracker.Securities;
using ProjectTracker.Utilities;
using ProjectTracker.ViewModels;

namespace ProjectTracker.Controllers
{
  [Authorize(Policy = "SuperAdmin")]
  public class AdminController : Controller
  {
    private readonly IMember _member;
    public AdminController(IMember member)
    {
      _member = member;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
      List<Member> members = await _member.GetAllMembersAsync();
      return View(members);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string memberId)
    {
      Member member = await _member.GetMemberByIdAsync(memberId);
      MemberEditViewModel editVM = new MemberEditViewModel
      {
        EncryptedId = memberId,
        FirstName = member.FirstName,
        LastName = member.LastName,
        Email = member.Email,
      };

      editVM.MemberClaims = new List<MemberClaim>();
      IList<Claim> memberClaims = await _member.GetMemberClaimsAsync(member);
      var claimTypes = Enum.GetValues(typeof(ClaimType));
      foreach (var claimType in claimTypes)
      {
        Claim claim = memberClaims.FirstOrDefault(mc => mc.Type == claimType.ToString());
        MemberClaim memberClaim = new MemberClaim();
        if (claim != null)
        {
          memberClaim.ClaimType = claim.Type;
          memberClaim.IsSelected = Convert.ToBoolean(claim.Value);
        }
        else
        {
          memberClaim.ClaimType = claimType.ToString();
          memberClaim.IsSelected = false;
        }

        editVM.MemberClaims.Add(memberClaim);
      }

      return View(editVM);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(MemberEditViewModel model)
    {
      if (string.IsNullOrEmpty(model.FirstName) || 
          string.IsNullOrEmpty(model.LastName) || 
          string.IsNullOrEmpty(model.Email))
      {
        ModelState.AddModelError(string.Empty, "All fields are required");
        return View();
      }

      IdentityResult identityResult = await _member.UpdateUserInfo(model);
      if (identityResult == null)
      {
        ModelState.AddModelError(string.Empty, "Update failed");
        return View();
      }
      else
      {
        ModelState.AddModelError(string.Empty, "Update successful");
        return RedirectToAction("Edit", new { memberId = model.EncryptedId });
      }
    }

    [HttpPost]
    public async Task<IActionResult> EditAccessPermission(MemberEditViewModel memberEditVM)
    {
      IdentityResult identityResult = await _member.UpdateAccessPermission(memberEditVM);

      if (identityResult == null)
      {
        TempData["AccessPermission"] = "Updating user Access Permission failed";
        return RedirectToAction("Edit", new { memberId = memberEditVM.EncryptedId });
      }
      else
      {
        TempData["AccessPermission"] = "Updating user Access Permission Successful";
        return RedirectToAction("Edit", new { memberId = memberEditVM.EncryptedId });
      }
    }

    [HttpGet]
    public async Task<IActionResult> Delete(string memberId)
    {
      var identityResult = await _member.DeleteAsync(memberId);
      if (identityResult.Succeeded)
      {
        return RedirectToAction("Index");
      }
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(MemberEditViewModel editVM)
    {
      IdentityResult identityResult = await _member.UpdatePassword(editVM);
      if (identityResult.Succeeded)
      {
        return RedirectToAction("Index");
      }
      return View();
    }
  }
}