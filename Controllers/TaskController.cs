using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    private readonly IMember _member;
    public TaskController(IProject project, ITask task, ITaskStatus taskStatus, IMember member)
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
      
      taskVM.Tasks = _task.GetAllTasks().ToList();
      foreach (var task in taskVM.Tasks)
      {
        task.TaskStatus = _taskStatus.GetTaskStatus(task.StatusId);
      }

      taskVM.Tasks = taskVM.Tasks.Where(task => task.ProjectId == id).OrderBy(task => task.TaskStatus.OrderPriority).ToList();
      foreach (var task in taskVM.Tasks)
      {
        task.Member = _member.GetMember(task.MemberId);
        task.TaskStatus = _taskStatus.GetTaskStatus(task.StatusId);
      }

      taskVM.TaskStatus = _taskStatus.GetAllTaskStatus().ToList();
      
      taskVM.Members = _member.GetAllMembers().ToList();

      return View(taskVM);
    }

    [HttpGet("create")]
    public async Task<IActionResult> Create(int id)
    {
      TaskViewModel taskVM = new TaskViewModel();

      taskVM.Project = await _project.GetProjectAsync(id);
      
      var task = new ProjectTracker.Models.Task();
      task.ProjectId = id;
      task.StatusId = _taskStatus.GetDefaultTaskStatus();
      task.Deadline = DateTime.Now.AddDays(1).Date;
      taskVM.Tasks.Add(task);

      taskVM.TaskStatus = _taskStatus.GetAllTaskStatus().OrderBy(ts => ts.OrderPriority).ToList();

      taskVM.Members = _member.GetAllMembers().ToList();

      return View(taskVM);
    }

    [HttpGet("{taskId}/edit")]
    public ViewResult Edit(int taskId)
    {
      TaskViewModel taskVM = new TaskViewModel();
      taskVM.Tasks.Add(_task.GetTask(taskId));
      taskVM.TaskStatus = _taskStatus.GetAllTaskStatus().OrderBy(ts => ts.OrderPriority).ToList();
      taskVM.Members = _member.GetAllMembers().ToList();

      return View(taskVM);
    }

    [HttpGet("{taskId}/delete")]
    public RedirectToActionResult Delete(int taskId)
    {
      _task.Delete(taskId);
      return RedirectToAction("index");
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(int id, TaskViewModel newTaskVM)
    {
      foreach (var task in newTaskVM.Tasks)
      {
        ProjectTracker.Models.Task newTask = _task.Add(task);
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
        _task.Update(task);
      }

      Project project = await _project.GetProjectAsync(id);
      await _project.UpdateAsync(project);

      return RedirectToAction("index");
    }

  }
}