using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using ProjectTracker.Models;
using ProjectTracker.Securities;
using ProjectTracker.Utilities;
using ProjectTracker.ViewModels;
using Task = ProjectTracker.Models.Task;

namespace ProjectTracker.Controllers
{
  [Route("project")]
  public class ProjectController : Controller
  {
    private readonly IProject _project;
    private readonly IAuthorizationService _authService;
    private readonly IProjectMember _projectMember;
    private readonly ITask _task;
    private readonly ITaskMember _taskMember;
    private readonly IMember _member;

    public ProjectController(IProject project, IAuthorizationService authService, IProjectMember projectMember, ITask task, ITaskMember taskMember, IMember member)
    {
      _project = project;
      _authService = authService;
      _projectMember = projectMember;
      _task = task;
      _taskMember = taskMember;
      _member = member;
    }

    [HttpGet("/projects")]
    public async Task<IActionResult> Index()
    {
      List<Project> projects = new List<Project>();
      List<int> projectIds = new List<int>();

      if ((await _authService.AuthorizeAsync(User, "SuperAdmin")).Succeeded)
      {
        projects = await _project.GetAllProjectsAsync();
      }
      else 
      {
        string memberId = _member.GetMemberId(User);

        projects = await _project.GetProjectsByMemberIdAsync(memberId);

        var projectMembers = await _projectMember.GetByMemberIdAsync(memberId);

        foreach (var projectMember in projectMembers)
        {
          var project = await _project.GetProjectByIdAsync(projectMember.ProjectId);
          projects.Add(project);
        }
      }

      ViewBag.individualTasks = (await _taskMember.GetByMemberIdAsync(_member.GetMemberId(User))).Count();

      return View(projects);
    }

    [HttpGet("create")]
    public IActionResult Create() => View();

    [HttpPost("create")]
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
        MemberId = _member.GetMemberId(User),
        ProjectName = newProjectVM.ProjectName,
        Summary = newProjectVM.Summary,
      };
      await _project.AddAsync(project);
      return RedirectToAction("Index");
    }

    [HttpGet("{projectId}/edit")]
    [Authorize(Policy = "CanAccessActions")]
    public async Task<IActionResult> Edit(string projectId)
    {
      int id = Convert.ToInt32(_project.UnprotectProjectId(projectId));

      return View(await GenerateProjectViewModel(id));
    }

    [HttpPost("{projectId}/edit")]
    [Authorize(Policy = "CanAccessActions")]
    public async Task<IActionResult> Edit(ProjectEditViewModel editProjectVM)
    {
      editProjectVM.Project.Id = Convert.ToInt32(_project.UnprotectProjectId(editProjectVM.Project.EncryptedId));

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

      editProjectVM.Project.Id = Convert.ToInt32(_project.UnprotectProjectId(editProjectVM.Project.EncryptedId));
      
      editProjectVM.Project.MemberId = _member.UnprotectMemberId(editProjectVM.Project.EncryptedMemberId);
      
      await _project.UpdateAsync(editProjectVM.Project);

      if (editProjectVM.ProjectMemberIdsToAdd.Any())
      {
        int projId = Convert.ToInt32(_project.UnprotectProjectId(editProjectVM.Project.EncryptedId));
        var projMemIdsToAdd = editProjectVM.ProjectMemberIdsToAdd
          .Select(projectMemberIdToAdd => {
            string id = _member.UnprotectMemberId(projectMemberIdToAdd);
            return id;
          })
          .ToList();
        await _projectMember.AddAsync(projId, projMemIdsToAdd);

        List<Task> projectTasks = await _task.GetAllTasksOfProjectIdAsync(editProjectVM.Project.Id);

        List<int> taskIds = new List<int>();
        
        projectTasks.ForEach(pt => taskIds.Add(pt.Id));

        editProjectVM.ProjectMemberIdsToAdd.ForEach(id => _taskMember.RemoveMemberFromTasks(taskIds, id));
      }

      if (editProjectVM.ProjectMemberIdsToRemove.Any())
      {
        int projId = Convert.ToInt32(_project.UnprotectProjectId(editProjectVM.Project.EncryptedId));
        var projMemIdsToRemove = editProjectVM.ProjectMemberIdsToRemove
          .Select(projectMemberIdToRemove => {
            string id = _member.UnprotectMemberId(projectMemberIdToRemove);
            return id;
          })
          .ToList();
        await _projectMember.RemoveAsync(projId, projMemIdsToRemove);
      }

      return RedirectToAction("Index");
    }

    [HttpPost("{projectId}/delete")]
    [Authorize(Policy = "CanAccessActions")]
    public async Task<IActionResult> Delete(string projectId)
    {
      int projId = Convert.ToInt32(_project.UnprotectProjectId(projectId));
      var project = await _project.GetProjectByIdAsync(projId);

      var proj = await _project.DeleteAsync(projId);
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

      projectVM.Members = await _member.GetAllMembersAsync();

      if (projectId != null)
      {
        projectVM.Project = await _project.GetProjectByIdAsync((int)projectId);

        if (projectVM.Project.MemberId != null)
        {
          projectVM.Project.Member = projectVM.Members.FirstOrDefault(m => m.Id == projectVM.Project.MemberId);
          projectVM.Project.EncryptedMemberId = projectVM.Project.Member.EncryptedId;
        }
      }

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