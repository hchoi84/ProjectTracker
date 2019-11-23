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
    public IActionResult Register()
    {
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(AdminRegisterViewModel regVM)
    {
      if (!ModelState.IsValid)
      {
        return View();
      }

      var result = await _member.RegisterAsync(regVM);

      if (result.Succeeded)
      {
        return RedirectToAction("Index");
      }

      foreach (var error in result.Errors)
      {
        ModelState.AddModelError(string.Empty, error.Description);
      }

      return View();
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string userId)
    {
      Member member = await _member.GetMemberAsync(userId);
      AdminEditViewModel editVM = new AdminEditViewModel
      {
        Id = member.Id,
        FirstName = member.FirstName,
        LastName = member.LastName,
        Email = member.Email,
        IsPswdNull = member.PasswordHash == null,
      };

      return View(editVM);
    }
  }
}