using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectTracker.Models;
using ProjectTracker.ViewModels;
using ProjectTracker.Utilities;
using Task = ProjectTracker.Models.Task;

namespace ProjectTracker.Controllers
{
  [Route("project/{projectId}")]
  public class TaskController : Controller
  {
    private readonly IProject _project;
    private readonly ITask _task;
    private readonly ITaskStatus _taskStatus;
    private readonly UserManager<Member> _member;
    private readonly IAuthorizationService _authService;
    private readonly ITaskMember _taskMember;
    private readonly IProjectMember _projectMember;
    public TaskController(IProject project, ITask task, ITaskStatus taskStatus, UserManager<Member> member, IAuthorizationService authService, ITaskMember taskMember, IProjectMember projectMember)
    {
      _project = project;
      _task = task;
      _taskStatus = taskStatus;
      _member = member;
      _authService = authService;
      _taskMember = taskMember;
      _projectMember = projectMember;
    }

    [HttpGet("tasks")]
    public async Task<IActionResult> Index(int projectId)
    {
      TaskViewModel taskVM = new TaskViewModel(projectId);
      taskVM.Tasks = await _task.GetAllTasksOfProjectIdAsync(projectId);
      taskVM.Project = await _project.GetProjectByIdAsync(projectId);

      return View(taskVM);
    }

    [HttpGet("tasks/create")]
    public async Task<IActionResult> Create(int projectId)
    {
      var taskStatuses = await _taskStatus.GetDefaultTaskStatusAsync();
      if (taskStatuses == 0)
      {
        ViewBag.ErrorTitle = "No Task Statuses or Default Task Status";
        ViewBag.ErrorMessage = "Please create Task Statuses or set a default Task Status by clicking " +
        "<a href=\"/TaskStatus\" class=\"text-primary\"><u>here</u></a>";
        return View("Error");
      }

      TaskCreateViewModel taskVM = new TaskCreateViewModel();

      taskVM.Task.StatusId = taskStatuses;
      taskVM.TaskStatuses = await _taskStatus.GetAllTaskStatusAsync();

      return View(taskVM);
    }

    [HttpPost("tasks/create")]
    public async Task<IActionResult> Create(int projectId, TaskCreateViewModel newTaskVM)
    {
      newTaskVM.Task.MemberId = _member.GetUserId(User);
      newTaskVM.Task.ProjectId = projectId;

      Task newTask = await _task.AddAsync(newTaskVM.Task);

      Project project = await _project.GetProjectByIdAsync(projectId);
      await _project.UpdateAsync(project);

      return RedirectToAction("Index", new { projectId = projectId });
    }

    [HttpGet("tasks/{taskId}/edit")]
    // TODO: implement [Authorize(Policy = "CanAccessActions)]
    public async Task<IActionResult> Edit(int taskId)
    {
      var task = await _task.GetTaskAsync(taskId);
      if (!(await _authService.AuthorizeAsync(User, "SuperAdmin")).Succeeded
        && task.MemberId != _member.GetUserId(User))
      {
        return RedirectToAction("Home", "AccessDenied");
      }

      TaskCreateViewModel taskVM = new TaskCreateViewModel();
      taskVM.Task = task;
      taskVM.TaskStatuses = await _taskStatus.GetAllTaskStatusAsync();

      (await _projectMember.GetAllMembersForProjectAsync(task.ProjectId))
        .ForEach(pm => taskVM.MembersPartOfProject.Add(pm.Member));

      taskVM.MembersAvailableToAdd = (await _member.Users.ToListAsync())
        .Where(m => m.Id != task.MemberId)
        .ToList();
      List<TaskMember> taskMembers = await _taskMember.GetAllMembersForTaskAsync(taskId);

      taskMembers.ForEach(tm => taskVM.MembersAvailableToRemove.Add(tm.Member));

      foreach (Member memberAvailableToAdd in taskVM.MembersAvailableToAdd.ToList())
      {
        Member projectMember = taskVM.MembersPartOfProject.FirstOrDefault(pm => pm.Id == memberAvailableToAdd.Id);
        if (projectMember != null)
        {
          taskVM.MembersAvailableToAdd.Remove(memberAvailableToAdd);
          continue;
        }

        TaskMember taskMember = taskMembers.FirstOrDefault(tm => tm.MemberId == memberAvailableToAdd.Id);
        if (taskMember != null)
        {
          taskVM.MembersAvailableToAdd.Remove(memberAvailableToAdd);
          taskMembers.Remove(taskMember);
        }
      }

      taskVM.MembersAvailableToAdd = taskVM.MembersAvailableToAdd.OrderBy(m => m.GetFullName).ToList();
      taskVM.MembersAvailableToRemove = taskVM.MembersAvailableToRemove.OrderBy(m => m.GetFullName).ToList();

      return View(taskVM);
    }

    [HttpPost("tasks/{taskId}/edit")]
    // TODO: implement [Authorize(Policy = "CanAccessActions)]
    public async Task<IActionResult> Edit(int projectId, int taskId, TaskCreateViewModel editTaskVM)
    {
      await _task.UpdateAsync(editTaskVM.Task);

      Project project = await _project.GetProjectByIdAsync(projectId);
      await _project.UpdateAsync(project);

      // TODO: implement the functionality to Add/Remove TaskMembers
      await _taskMember.AddMembersAsync(taskId, editTaskVM.TaskMemberIdsToAdd);
      await _taskMember.RemoveMembersAsync(taskId, editTaskVM.TaskMemberIdsToRemove);

      return RedirectToAction("Index", new { projectId = projectId });
    }

    [HttpPost("tasks/{taskId}/delete")]
    // TODO: implement [Authorize(Policy = "CanAccessActions)]
    public async Task<RedirectToActionResult> Delete(int projectId, int taskId)
    {
      var task = await _task.GetTaskAsync(taskId);
      if (!(await _authService.AuthorizeAsync(User, "SuperAdmin")).Succeeded
        && task.MemberId != _member.GetUserId(User))
      {
        return RedirectToAction("Home", "AccessDenied");
      }

      await _task.DeleteAsync(taskId);
      return RedirectToAction("index", new { projectId = projectId });
    }

    [HttpPost("/")]
    // Summary: retrieves and stores data in Session as Object to prevent URL clutter
    public async Task<IActionResult> GetTasksByMembersAndStoreInSession(HomeIndexViewModel model)
    {
      List<int> projectIds = new List<int>();
      List<Task> tasks = new List<Task>();

      foreach(string memberId in model.MemberIds)
      {
        // Retrieve project IDs the member has created from Projects
        (await _project.GetProjectsByMemberId(memberId))
          .ForEach(project => projectIds.Add(project.Id));

        // Retrieve project IDs the member is part of from ProjectMembers
        _projectMember.GetByMemberId(memberId)
          .ForEach(pm => projectIds.Add(pm.ProjectId));

        _taskMember.GetByMemberId(memberId)
          .ForEach(tm => tasks.Add(tm.Task));
      }

      projectIds = projectIds.Distinct().ToList();

      projectIds.ForEach(projectId =>
        _task.GetAllTasksOfProjectId(projectId).ForEach(task =>
          tasks.Add(task)));
      
      HttpContext.Session.SetObject("TBM", tasks);

      return RedirectToAction("TasksByMembers");
    }

    [HttpGet("/tasks")]
    public IActionResult TasksByMembers()
    {
      List<Task> tasks = HttpContext.Session.GetObject<List<Task>>("TBM");
      HttpContext.Session.Clear();

      return View("DisplayTasks", tasks);
    }
  }
}