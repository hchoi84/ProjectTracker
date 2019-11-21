using System;
using Microsoft.AspNetCore.Mvc;
using ProjectTracker.Models;
using ProjectTracker.ViewModels;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ProjectTracker.Controllers
{
  [AllowAnonymous]
  public class HomeController : Controller
  {
    private readonly IProject _project;
    private readonly UserManager<Member> _member;
    public HomeController(IProject project, UserManager<Member> member)
    {
      _member = member;
      _project = project;
    }

    public async Task<IActionResult> Index()
    {
      var projects = await _project.GetAllProjectsAsync();

      return View(projects);
    }

    [HttpGet("/project/create")]
    public async Task<IActionResult> Create()
    {
      var projectVM = new ProjectViewModel();
      projectVM.Members = await _member.Users.ToListAsync();
      return View(projectVM);
    }

    [HttpGet("/project/{id}/edit")]
    public async Task<ViewResult> Edit(int id)
    {
      var projectVM = new ProjectViewModel();
      projectVM.Project = await _project.GetProjectAsync(id);
      projectVM.Members = await _member.Users.ToListAsync();
      return View(projectVM);
    }

    [HttpGet("/project/{id}/delete")]
    public async Task<RedirectToActionResult> Delete(int id)
    {
      await _project.DeleteAsync(id);
      return RedirectToAction("index");
    }

    [HttpPost("/project/create")]
    public async Task<IActionResult> Create(ProjectViewModel newProjectVM)
    {
      if (ModelState.IsValid && await _project.IsUnique(newProjectVM.Project.ProjectName))
      {
        await _project.AddAsync(newProjectVM.Project);
        return RedirectToAction("Index");
      }

      if (!await _project.IsUnique(newProjectVM.Project.ProjectName))
      {
        ModelState.AddModelError(string.Empty, "Project Name already exists");
      }

      var projectVM = new ProjectViewModel();
      projectVM.Members = await _member.Users.ToListAsync();
      return View(projectVM);
    }

    [HttpPost("/project/{id}/edit")]
    public async Task<IActionResult> Edit(int id, ProjectViewModel editProjectVM)
    {
      if (ModelState.IsValid && await _project.IsUnique(editProjectVM.Project.ProjectName, id))
      {
        await _project.UpdateAsync(editProjectVM.Project);
        return RedirectToAction("Index");
      }

      if (!await _project.IsUnique(editProjectVM.Project.ProjectName, id))
      {
        ModelState.AddModelError(string.Empty, "Project Name already exists");
      }

      var projectVM = new ProjectViewModel();
      projectVM.Project = await _project.GetProjectAsync(id);
      projectVM.Members = await _member.Users.ToListAsync();
      return View(projectVM);
    }
  }
}