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
  public class ProjectController : Controller
  {
    private readonly IProject _project;
    private readonly UserManager<Member> _member;
    private readonly IAuthorizationService _authService;
    public ProjectController(IProject project, UserManager<Member> member, IAuthorizationService authService)
    {
      _project = project;
      _member = member;
      _authService = authService;
    }

    public async Task<IActionResult> Index()
    {
      var projects = await _project.GetAllProjectsAsync();

      return View(projects);
    }

    [HttpGet]
    public IActionResult Create() => View();

    [HttpPost]
    public async Task<IActionResult> Create(ProjectCreateViewModel newProjectVM)
    {
      if (ModelState.IsValid)
      {
        newProjectVM.ProjectName = newProjectVM.ProjectName.TrimAndTitleCase();
      }
      else
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

    [HttpGet]
    public async Task<IActionResult> Edit(int projectId)
    {
      var project = await _project.GetProjectAsync(projectId);

      if (!(await _authService.AuthorizeAsync(User, "SuperAdmin")).Succeeded
        && project.MemberId != _member.GetUserId(User))
      {
        return RedirectToAction("Home", "AcceessDenied");
      }

      return View(await GenerateProjectViewModel(projectId));
    }

    [HttpPost]
    public async Task<IActionResult> Edit(ProjectEditViewModel editProjectVM)
    {
      if (ModelState.IsValid)
      {
        editProjectVM.Project.ProjectName = editProjectVM.Project.ProjectName.TrimAndTitleCase();
      }
      else
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

    [HttpPost]
    public async Task<IActionResult> Delete(int projectId)
    {
      var project = await _project.GetProjectAsync(projectId);

      if (!(await _authService.AuthorizeAsync(User, "SuperAdmin")).Succeeded
        && project.MemberId != _member.GetUserId(User))
      {
        return RedirectToAction("Home", "AccessDenied");
      }

      var proj = await _project.DeleteAsync(projectId);
      if (proj == null)
      {
        ViewBag.ErrorMessage = "You can't delete as there are tasks associated with it";
        return View("Error");
      }
      return RedirectToAction("index");
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