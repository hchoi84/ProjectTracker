using System;
using Microsoft.AspNetCore.Mvc;
using ProjectTracker.Models;
using ProjectTracker.ViewModels;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectTracker.Utilities;

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
      return View(await GenerateProjectViewModel(null));
    }

    [HttpGet("/project/{id}/edit")]
    public async Task<ViewResult> Edit(int id)
    {
      return View(await GenerateProjectViewModel(id));
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
      newProjectVM.Project.ProjectName = newProjectVM.Project.ProjectName.TrimAndTitleCase();

      if (!ModelState.IsValid)
      {
        return View(await GenerateProjectViewModel(null));
      }

      if (!await _project.IsUnique(newProjectVM.Project.ProjectName))
      {
        ModelState.AddModelError(string.Empty, "Project Name already exists");
        return View(await GenerateProjectViewModel(null));
      }

      await _project.AddAsync(newProjectVM.Project);
      return RedirectToAction("Index");
    }

    [HttpPost("/project/{id}/edit")]
    public async Task<IActionResult> Edit(ProjectViewModel editProjectVM)
    {
      editProjectVM.Project.ProjectName = editProjectVM.Project.ProjectName.TrimAndTitleCase();

      if (!ModelState.IsValid)
      {
        return View(await GenerateProjectViewModel(editProjectVM.Project.Id));
      }

      if (!await _project.IsUnique(editProjectVM.Project.ProjectName, editProjectVM.Project.Id))
      {
        ModelState.AddModelError(string.Empty, "Project Name already exists");
        return View(await GenerateProjectViewModel(editProjectVM.Project.Id));
      }

      await _project.UpdateAsync(editProjectVM.Project);
      return RedirectToAction("Index");
    }

    public async Task<ProjectViewModel> GenerateProjectViewModel(int? id)
    {
      var projectVM = new ProjectViewModel();
      if (id != null)
      {
        projectVM.Project = await _project.GetProjectAsync((int)id);
      }
      projectVM.Members = await _member.Users.ToListAsync();

      return projectVM;
    }
  }
}