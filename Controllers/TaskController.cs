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

namespace ProjectTracker.Controllers
{
  [AllowAnonymous]
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
      TaskViewModel taskVM = new TaskViewModel();
      taskVM.Project = await _project.GetProjectAsync(id);
      
      taskVM.Tasks = await _task.GetAllTasksAsync();
      foreach (var task in taskVM.Tasks)
      {
        task.TaskStatus = await _taskStatus.GetTaskStatusAsync(task.StatusId);
      }

      taskVM.Tasks = taskVM.Tasks.Where(task => task.ProjectId == id).OrderBy(task => task.TaskStatus.OrderPriority).ToList();
      foreach (var task in taskVM.Tasks)
      {
        task.Member = await _member.FindByIdAsync(task.MemberId);
        task.TaskStatus = await _taskStatus.GetTaskStatusAsync(task.StatusId);
      }

      taskVM.TaskStatus = await _taskStatus.GetAllTaskStatusAsync();
      
      taskVM.Members = await _member.Users.ToListAsync();

      return View(taskVM);
    }

    [HttpGet("create")]
    public async Task<IActionResult> Create(int id)
    {
      TaskViewModel taskVM = new TaskViewModel();

      taskVM.Project = await _project.GetProjectAsync(id);
      
      var task = new ProjectTracker.Models.Task();
      task.ProjectId = id;
      task.StatusId = await _taskStatus.GetDefaultTaskStatusAsync();
      taskVM.Tasks.Add(task);

      taskVM.TaskStatus = await _taskStatus.GetAllTaskStatusAsync();

      taskVM.Members = await _member.Users.ToListAsync();

      return View(taskVM);
    }

    [HttpGet("{taskId}/edit")]
    public async Task<ViewResult> Edit(int taskId)
    {
      TaskViewModel taskVM = new TaskViewModel();
      taskVM.Tasks.Add(await _task.GetTaskAsync(taskId));
      taskVM.TaskStatus = await _taskStatus.GetAllTaskStatusAsync();
      taskVM.Members = await _member.Users.ToListAsync();

      return View(taskVM);
    }

    [HttpGet("{taskId}/delete")]
    public async Task<RedirectToActionResult> Delete(int taskId)
    {
      await _task.DeleteAsync(taskId);
      return RedirectToAction("index");
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(int id, TaskViewModel newTaskVM)
    {
      foreach (var task in newTaskVM.Tasks)
      {
        ProjectTracker.Models.Task newTask = await _task.AddAsync(task);
      }
      
      Project project = await _project.GetProjectAsync(id);
      await _project.UpdateAsync(project);

      return RedirectToAction("Index");
    }

    [HttpPost("{taskId}/edit")]
    public async Task<IActionResult> Edit(int id, TaskViewModel editTaskVM)
    {
      foreach (var task in editTaskVM.Tasks)
      {
        await _task.UpdateAsync(task);
      }

      Project project = await _project.GetProjectAsync(id);
      await _project.UpdateAsync(project);

      return RedirectToAction("index");
    }

  }
}