using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectTracker.Models;
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
    public async Task<IActionResult> Edit(string userId)
    {
      Member member = await _member.GetMemberByIdAsync(userId);
      MemberEditViewModel editVM = new MemberEditViewModel
      {
        Id = member.Id,
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
      IdentityResult identityResult = await _member.UpdateUserInfo(model);
      if (identityResult == null)
      {
        ViewBag.Message = "Update failed";
        return View();
      }
      else
      {
        ViewBag.Message = "Updated successful";
        return RedirectToAction("Index");
      }
    }

    [HttpPost]
    public async Task<IActionResult> EditAccessPermission(MemberEditViewModel memberEditVM)
    {
      IdentityResult identityResult = await _member.UpdateAccessPermission(memberEditVM);

      if (identityResult == null)
      {
        ModelState.AddModelError(string.Empty, "Updating user Access Permission failed");
        return RedirectToAction("Edit", new { userId = memberEditVM.Id });
      }
      else
      {
        ModelState.AddModelError(string.Empty, "Updating user Access Permission successful");
        TempData["AccessPermissionSuccess"] = "Updating user Access Permission Successful";
        return RedirectToAction("Edit", new { userId = memberEditVM.Id });
      }
    }

    [HttpGet]
    public async Task<IActionResult> Delete(string userId)
    {
      var identityResult = await _member.DeleteAsync(userId);
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