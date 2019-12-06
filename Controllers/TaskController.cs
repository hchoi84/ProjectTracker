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
  [Route("project/{id}/tasks")]
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

    [HttpGet("")]
    public async Task<IActionResult> Index(int id)
    {
      TaskViewModel taskVM = new TaskViewModel(id);
      taskVM.Tasks = await _task.GetAllTasksOfProjectIdAsync(id);
      taskVM.Project = await _project.GetProjectAsync(id);

      return View(taskVM);
    }

    [HttpGet("create")]
    public async Task<IActionResult> Create(int id)
    {
      TaskCreateViewModel taskVM = new TaskCreateViewModel();

      taskVM.Task.StatusId = await _taskStatus.GetDefaultTaskStatusAsync();
      taskVM.TaskStatuses = await _taskStatus.GetAllTaskStatusAsync();

      return View(taskVM);
    }

    [HttpGet("{taskId}/edit")]
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

    [HttpGet("{taskId}/delete")]
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

    [HttpPost("create")]
    public async Task<IActionResult> Create(int id, TaskCreateViewModel newTaskVM)
    {
      newTaskVM.Task.MemberId = _member.GetUserId(User);
      newTaskVM.Task.ProjectId = id;

      Task newTask = await _task.AddAsync(newTaskVM.Task);
      
      Project project = await _project.GetProjectAsync(id);
      await _project.UpdateAsync(project);

      return RedirectToAction("Index");
    }

    [HttpPost("{taskId}/edit")]
    public async Task<IActionResult> Edit(int id, TaskCreateViewModel editTaskVM)
    {
      await _task.UpdateAsync(editTaskVM.Task);

      Project project = await _project.GetProjectAsync(id);
      await _project.UpdateAsync(project);

      return RedirectToAction("index");
    }

  }
}