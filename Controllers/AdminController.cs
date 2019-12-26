using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectTracker.Models;
using ProjectTracker.ViewModels;

namespace ProjectTracker.Controllers
{
  [AllowAnonymous]
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

      return View(editVM);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(MemberEditViewModel model)
    {
      IdentityResult identityResult = await _member.UpdateAsync(model);
      if (identityResult == null)
      {
        ViewBag.Message = "Update failed";
        return View();
      }
      else
      {
        ViewBag.Message = "Updated successfully";
        return RedirectToAction("Index");
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