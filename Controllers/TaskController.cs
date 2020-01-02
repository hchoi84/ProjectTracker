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
    public TaskController(IProject project, ITask task, ITaskStatus taskStatus, UserManager<Member> member)
    {
      _member = member;
      _project = project;
      _task = task;
      _taskStatus = taskStatus;
    }

    [HttpGet("tasks")]
    public async Task<IActionResult> Index(int projectId)
    {
      TaskViewModel taskVM = new TaskViewModel(projectId);
      taskVM.Tasks = await _task.GetAllTasksOfProjectIdAsync(projectId);
      taskVM.Project = await _project.GetProjectAsync(projectId);

      return View(taskVM);
    }

    [HttpGet("tasks/create")]
    public async Task<IActionResult> Create(int projectId)
    {
      TaskCreateViewModel taskVM = new TaskCreateViewModel();

      taskVM.Task.StatusId = await _taskStatus.GetDefaultTaskStatusAsync();
      taskVM.TaskStatuses = await _taskStatus.GetAllTaskStatusAsync();

      return View(taskVM);
    }
    
    [HttpPost("tasks/create")]
    public async Task<IActionResult> Create(int projectId, TaskCreateViewModel newTaskVM)
    {
      newTaskVM.Task.MemberId = _member.GetUserId(User);
      newTaskVM.Task.ProjectId = projectId;

      Task newTask = await _task.AddAsync(newTaskVM.Task);
      
      Project project = await _project.GetProjectAsync(projectId);
      await _project.UpdateAsync(project);

      return RedirectToAction("Index", new { projectId = projectId });
    }

    [HttpGet("tasks/{taskId}/edit")]
    public async Task<IActionResult> Edit(int taskId)
    {
      var task = await _task.GetTaskAsync(taskId);
      if (task.MemberId != _member.GetUserId(User))
      {
        return RedirectToAction("Error", "Error");
      }

      TaskCreateViewModel taskVM = new TaskCreateViewModel();
      taskVM.Task = await _task.GetTaskAsync(taskId);
      taskVM.TaskStatuses = await _taskStatus.GetAllTaskStatusAsync();

      return View(taskVM);
    }

    [HttpPost("tasks/{taskId}/delete")]
    public async Task<RedirectToActionResult> Delete(int projectId, int taskId)
    {
      var task = await _task.GetTaskAsync(taskId);
      if (task.MemberId != _member.GetUserId(User))
      {
        return RedirectToAction("Error", "Error");
      }

      await _task.DeleteAsync(taskId);
      return RedirectToAction("index", new { projectId = projectId });
    }

    [HttpPost("tasks/{taskId}/edit")]
    public async Task<IActionResult> Edit(int projectId, TaskCreateViewModel editTaskVM)
    {
      await _task.UpdateAsync(editTaskVM.Task);

      Project project = await _project.GetProjectAsync(projectId);
      await _project.UpdateAsync(project);

      return RedirectToAction("Index", new { projectId = projectId });
    }

    [HttpPost("/")]
    public async Task<IActionResult> GetTasksByMembers(HomeIndexViewModel model)
    {
      List<Task> tasks = await _task.GetTasksByMemberIds(model.MemberIds);
      HttpContext.Session.SetObject("TBM", tasks);

      return RedirectToAction("TasksByMembers");
    }

    [HttpGet("/tasks")]
    public IActionResult TasksByMembers()
    {
      List<Task> tasks = HttpContext.Session.GetObject<List<Task>>("TBM");

      return View("DisplayTasks", tasks);
    }
  }
}