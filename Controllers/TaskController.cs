using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ProjectTracker.Models;
using ProjectTracker.ViewModels;

namespace ProjectTracker.Controllers
{
  [Route("project/{id}/tasks")]
  public class TaskController : Controller
  {
    private readonly IProject _project;
    private readonly ITask _task;
    private readonly ITaskStatus _taskStatus;
    private readonly IUser _user;
    public TaskController(IProject project, ITask task, ITaskStatus taskStatus, IUser user)
    {
      _user = user;
      _project = project;
      _task = task;
      _taskStatus = taskStatus;
    }

    [HttpGet("")]
    public IActionResult Index(int id)
    {
      TaskViewModel taskVM = new TaskViewModel();
      taskVM.Project = _project.GetProject(id);
      
      taskVM.Tasks = _task.GetAllTasks().ToList();
      foreach (var task in taskVM.Tasks)
      {
        task.TaskStatus = _taskStatus.GetTaskStatus(task.StatusId);
      }

      taskVM.Tasks = taskVM.Tasks.Where(task => task.ProjectId == id).OrderBy(task => task.TaskStatus.OrderPriority).ToList();
      foreach (var task in taskVM.Tasks)
      {
        task.Creator = _user.GetUser(task.UserId);
        task.TaskStatus = _taskStatus.GetTaskStatus(task.StatusId);
      }

      taskVM.TaskStatus = _taskStatus.GetAllTaskStatus().ToList();
      
      taskVM.Users = _user.GetAllUsers().ToList();

      return View(taskVM);
    }

    [HttpGet("create")]
    public IActionResult Create(int id)
    {
      TaskViewModel taskVM = new TaskViewModel();

      taskVM.Project = _project.GetProject(id);
      
      var task = new Task();
      task.ProjectId = id;
      task.StatusId = _taskStatus.GetDefaultTaskStatus();
      task.Deadline = DateTime.Now.AddDays(1).Date;
      taskVM.Tasks.Add(task);

      taskVM.TaskStatus = _taskStatus.GetAllTaskStatus().OrderBy(ts => ts.OrderPriority).ToList();

      taskVM.Users = _user.GetAllUsers().ToList();

      return View(taskVM);
    }

    [HttpGet("{taskId}/edit")]
    public ViewResult Edit(int taskId)
    {
      TaskViewModel taskVM = new TaskViewModel();
      taskVM.Tasks.Add(_task.GetTask(taskId));
      taskVM.TaskStatus = _taskStatus.GetAllTaskStatus().OrderBy(ts => ts.OrderPriority).ToList();
      taskVM.Users = _user.GetAllUsers().ToList();

      return View(taskVM);
    }

    [HttpGet("{taskId}/delete")]
    public RedirectToActionResult Delete(int taskId)
    {
      _task.Delete(taskId);
      return RedirectToAction("index");
    }

    [HttpPost("create")]
    public IActionResult Create(int id, TaskViewModel newTaskVM)
    {
      foreach (var task in newTaskVM.Tasks)
      {
        Task newTask = _task.Add(task);
      }
      
      Project project = _project.GetProject(id);
      _project.Update(project);

      return RedirectToAction("Index");
    }

    [HttpPost("{taskId}/edit")]
    public IActionResult Edit(int id, TaskViewModel editTaskVM)
    {
      foreach (var task in editTaskVM.Tasks)
      {
        _task.Update(task);
      }

      Project project = _project.GetProject(id);
      _project.Update(project);

      return RedirectToAction("index");
    }

  }
}