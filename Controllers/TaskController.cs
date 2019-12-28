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
using Task = ProjectTracker.Models.Task;

namespace ProjectTracker.Controllers
{
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

    [HttpGet("project/{id}/tasks")]
    public async Task<IActionResult> Index(int id)
    {
      TaskViewModel taskVM = new TaskViewModel(id);
      taskVM.Tasks = await _task.GetAllTasksOfProjectIdAsync(id);
      taskVM.Project = await _project.GetProjectAsync(id);

      return View(taskVM);
    }

    [HttpGet("project/{id}/tasks/create")]
    public async Task<IActionResult> Create(int id)
    {
      TaskCreateViewModel taskVM = new TaskCreateViewModel();

      taskVM.Task.StatusId = await _taskStatus.GetDefaultTaskStatusAsync();
      taskVM.TaskStatuses = await _taskStatus.GetAllTaskStatusAsync();

      return View(taskVM);
    }

    [HttpGet("project/{id}/tasks/{taskId}/edit")]
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

    [HttpGet("project/{id}/tasks/{taskId}/delete")]
    public async Task<RedirectToActionResult> Delete(int taskId)
    {
      var task = await _task.GetTaskAsync(taskId);
      if (task.MemberId != _member.GetUserId(User))
      {
        return RedirectToAction("Error", "Error");
      }

      await _task.DeleteAsync(taskId);
      return RedirectToAction("index");
    }

    [HttpPost("project/{id}/tasks/create")]
    public async Task<IActionResult> Create(int id, TaskCreateViewModel newTaskVM)
    {
      newTaskVM.Task.MemberId = _member.GetUserId(User);
      newTaskVM.Task.ProjectId = id;

      Task newTask = await _task.AddAsync(newTaskVM.Task);
      
      Project project = await _project.GetProjectAsync(id);
      await _project.UpdateAsync(project);

      return RedirectToAction("Index");
    }

    [HttpPost("project/{id}/tasks/{taskId}/edit")]
    public async Task<IActionResult> Edit(int id, TaskCreateViewModel editTaskVM)
    {
      await _task.UpdateAsync(editTaskVM.Task);

      Project project = await _project.GetProjectAsync(id);
      await _project.UpdateAsync(project);

      return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> TasksByMembers(HomeIndexViewModel model)
    {
      List<Task> tasks = await _task.GetTasksByMemberIds(model.MemberIds);

      return View("DisplayTasks", tasks);
    }
  }
}