using Microsoft.AspNetCore.Mvc;
using ProjectTracker.Models;
using ProjectTracker.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectTracker.Utilities;

namespace ProjectTracker.Controllers
{
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
    public IActionResult Create() => View();

    [HttpGet("/project/{id}/edit")]
    public async Task<IActionResult> Edit(int id)
    {
      var project = await _project.GetProjectAsync(id);
      
      if (project.MemberId != _member.GetUserId(User))
      {
        return RedirectToAction("Error", "Error");
      }

      return View(await GenerateProjectViewModel(id));
    }

    [HttpGet("/project/{id}/delete")]
    public async Task<RedirectToActionResult> Delete(int id)
    {
      var project = await _project.GetProjectAsync(id);
      
      if (project.MemberId != _member.GetUserId(User))
      {
        return RedirectToAction("Error", "Error");
      }

      await _project.DeleteAsync(id);
      return RedirectToAction("index");
    }

    [HttpPost("/project/create")]
    public async Task<IActionResult> Create(ProjectCreateViewModel newProjectVM)
    {
      newProjectVM.ProjectName = newProjectVM.ProjectName.TrimAndTitleCase();

      if (!ModelState.IsValid)
      {
        return View();
      }

      if (!await _project.IsUnique(newProjectVM.ProjectName))
      {
        ModelState.AddModelError(string.Empty, "Project Name already exists");
        return View();
      }

      Project project = new Project()
      {
        Deadline = newProjectVM.Deadline,
        MemberId = _member.GetUserId(User),
        ProjectName = newProjectVM.ProjectName,
        Summary = newProjectVM.Summary,
      };
      await _project.AddAsync(project);
      return RedirectToAction("Index");
    }

    [HttpPost("/project/{id}/edit")]
    public async Task<IActionResult> Edit(ProjectEditViewModel editProjectVM)
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

    public async Task<ProjectEditViewModel> GenerateProjectViewModel(int? id)
    {
      var projectVM = new ProjectEditViewModel();
      if (id != null)
      {
        projectVM.Project = await _project.GetProjectAsync((int)id);
      }
      projectVM.Members = await _member.Users.ToListAsync();

      return projectVM;
    }
  }
}