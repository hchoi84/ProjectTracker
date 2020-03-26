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
using ProjectTracker.Securities;
using Microsoft.AspNetCore.DataProtection;

namespace ProjectTracker.Controllers
{
  [Route("project/{projectId}")]
  public class TaskController : Controller
  {
    private readonly IProject _project;
    private readonly ITask _task;
    private readonly ITaskStatus _taskStatus;
    private readonly IAuthorizationService _authService;
    private readonly ITaskMember _taskMember;
    private readonly IProjectMember _projectMember;
    private readonly IMember _member;

    public TaskController(IProject project, ITask task, ITaskStatus taskStatus, IAuthorizationService authService, ITaskMember taskMember, IProjectMember projectMember, IMember member)
    {
      _project = project;
      _task = task;
      _taskStatus = taskStatus;
      _authService = authService;
      _taskMember = taskMember;
      _projectMember = projectMember;
      _member = member;
    }

    [HttpGet("/individualtasks")]
    public async Task<IActionResult> Index()
    {
      TaskViewModel taskVM = new TaskViewModel();
      List<TaskMember> taskMembers = new List<TaskMember>();

      string memberId = _member.GetMemberId(User);
      taskMembers = await _taskMember.GetByMemberIdAsync(memberId);
      foreach (TaskMember taskMember in taskMembers)
      {
        Task task = await _task.GetTaskAsync(taskMember.TaskId);
        taskVM.Tasks.Add(task);
      }

      return View("IndividualTasks", taskVM);
    }

    [HttpGet("tasks")]
    [Authorize(Policy = "CanAccessTasks")]
    public async Task<IActionResult> Index(string projectId)
    {
      int projId = _project.UnprotectProjectId(projectId);

      TaskViewModel taskVM = new TaskViewModel();
      taskVM.Tasks = await _task.GetAllTasksOfProjectIdAsync(projId);
      taskVM.Project = await _project.GetProjectByIdAsync(projId);
      taskVM.Project.EncryptedId = _project.ProtectProjectId(taskVM.Project.Id);

      return View(taskVM);
    }

    [HttpGet("tasks/create")]
    [Authorize(Policy = "CanAccessActions")]
    public async Task<IActionResult> Create(string projectId)
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
      taskVM.ProjectId = projectId;

      return View(taskVM);
    }

    [HttpPost("tasks/create")]
    [Authorize(Policy = "CanAccessActions")]
    public async Task<IActionResult> Create(string projectId, TaskCreateViewModel newTaskVM)
    {
      int projId = _project.UnprotectProjectId(projectId);

      newTaskVM.Task.MemberId = _member.GetMemberId(User);
      newTaskVM.Task.ProjectId = projId;

      Task newTask = await _task.AddAsync(newTaskVM.Task);

      Project project = await _project.GetProjectByIdAsync(projId);
      await _project.UpdateAsync(project);

      return RedirectToAction("Index", new { projectId = projectId });
    }

    [HttpGet("tasks/{taskId}/edit")]
    [Authorize(Policy = "CanAccessActions")]
    public async Task<IActionResult> Edit(string projectId, string taskId)
    {
      int tId = _task.UnprotectTaskId(taskId);

      var task = await _task.GetTaskAsync(tId);

      TaskCreateViewModel taskVM = new TaskCreateViewModel();
      taskVM.ProjectId = projectId;
      taskVM.Task = task;
      taskVM.TaskStatuses = await _taskStatus.GetAllTaskStatusAsync();

      (await _projectMember.GetAllMembersForProjectAsync(task.ProjectId))
        .ForEach(pm => taskVM.MembersPartOfProject.Add(pm.Member));

      taskVM.MembersAvailableToAdd = (await _member.GetAllMembersAsync())
        .Where(m => m.Id != task.MemberId)
        .ToList();

      List<TaskMember> taskMembers = await _taskMember.GetAllMembersForTaskAsync(tId);

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
    [Authorize(Policy = "CanAccessActions")]
    public async Task<IActionResult> Edit(string projectId, string taskId, TaskCreateViewModel editTaskVM)
    {
      int tId = _task.UnprotectTaskId(taskId);

      int projId = _project.UnprotectProjectId(projectId);

      await _task.UpdateAsync(editTaskVM.Task);

      Project project = await _project.GetProjectByIdAsync(projId);
      await _project.UpdateAsync(project);

      await _taskMember.AddMembersAsync(tId, editTaskVM.TaskMemberIdsToAdd);
      await _taskMember.RemoveMembersAsync(tId, editTaskVM.TaskMemberIdsToRemove);

      return RedirectToAction("Index", new { projectId = projectId });
    }

    [HttpPost("tasks/{taskId}/delete")]
    [Authorize(Policy = "CanAccessActions")]
    public async Task<RedirectToActionResult> Delete(string projectId, string taskId)
    {
      int tId = _task.UnprotectTaskId(taskId);
      var task = await _task.GetTaskAsync(tId);

      await _task.DeleteAsync(tId);
      return RedirectToAction("index", new { projectId = projectId });
    }

    [HttpPost("/")]
    // Summary: retrieves and stores data in Session as Object to prevent URL clutter
    public async Task<IActionResult> GetTasksByMembersAndStoreInSession(HomeIndexViewModel model)
    {
      List<int> projectIds = new List<int>();
      List<int> taskIds = new List<int>();
      List<Task> tasks = new List<Task>();

      foreach(string memberId in model.MemberIds)
      {
        string memId = _member.UnprotectMemberId(memberId);

        (await _project.GetProjectsByMemberIdAsync(memId))
          .ForEach(project => projectIds.Add(project.Id));

        (await _projectMember.GetByMemberIdAsync(memId))
          .ForEach(pm => projectIds.Add(pm.ProjectId));

        (await _taskMember.GetByMemberIdAsync(memId))
          .ForEach(tm => taskIds.Add(tm.TaskId));
      }

      projectIds = projectIds.Distinct().ToList();

      (await _task.GetAllTasksOfProjectIdsAsync(projectIds))
        .ForEach(t => tasks.Add(t));

      (await _task.GetByTaskIdsAsync(taskIds))
        .ForEach(t => tasks.Add(t));

      tasks.ForEach(t => {
        t.EncryptedProjectId = _project.ProtectProjectId(t.ProjectId);
      });
      
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