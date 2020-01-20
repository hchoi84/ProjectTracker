using System.Collections.Generic;
using System.Linq;
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
    private readonly IProjectMember _projectMember;
    public ProjectController(IProject project, UserManager<Member> member, IAuthorizationService authService, IProjectMember projectMember)
    {
      _project = project;
      _member = member;
      _authService = authService;
      _projectMember = projectMember;
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
    [Authorize(Policy = "CanAccessActions")]
    public async Task<IActionResult> Edit(int projectId)
    {
      var project = await _project.GetProjectAsync(projectId);

      return View(await GenerateProjectViewModel(projectId));
    }

    [HttpPost]
    [Authorize(Policy = "CanAccessActions")]
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
      await _projectMember.AddAsync(editProjectVM.Project.Id, editProjectVM.ProjectMemberIdsToAdd);
      await _projectMember.RemoveAsync(editProjectVM.Project.Id, editProjectVM.ProjectMemberIdsToRemove);
      return RedirectToAction("Index");
    }

    [HttpPost]
    [Authorize(Policy = "CanAccessActions")]
    public async Task<IActionResult> Delete(int projectId)
    {
      var project = await _project.GetProjectAsync(projectId);

      var proj = await _project.DeleteAsync(projectId);
      if (proj == null)
      {
        ViewBag.ErrorMessage = "You can't delete as there are tasks associated with it";
        return View("Error");
      }
      return RedirectToAction("index");
    }

    public async Task<ProjectEditViewModel> GenerateProjectViewModel(int? projectId)
    {
      var projectVM = new ProjectEditViewModel();

      if (projectId != null)
      {
        projectVM.Project = await _project.GetProjectAsync((int)projectId);
      }

      projectVM.Members = await _member.Users.ToListAsync();

      List<ProjectMember> projectMembers = await _projectMember.GetAllMembersForProjectAsync((int)projectId);

      List<Member> tempMembersAvailableToAdd = projectVM.Members.Where(m => m.Id != projectVM.Project.MemberId).ToList();
      foreach (ProjectMember projectMember in projectMembers)
      {
        Member tempMemberAvailableToAdd = tempMembersAvailableToAdd.FirstOrDefault(temp => temp.Id == projectMember.MemberId);
        if (tempMemberAvailableToAdd != null)
        {
          tempMembersAvailableToAdd.Remove(tempMemberAvailableToAdd);
        }
      }
      projectVM.MembersAvailableToAdd = tempMembersAvailableToAdd;

      foreach (ProjectMember projectMember in projectMembers)
      {
        projectVM.MembersAvailableToRemove.Add(projectMember.Member);
      }

      return projectVM;
    }
  }
}