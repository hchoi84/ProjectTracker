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
    public TaskController(IProject project, ITask task, ITaskStatus taskStatus)
    {
      _project = project;
      _task = task;
      _taskStatus = taskStatus;
    }

    [HttpGet("")]
    public IActionResult Index(int id)
    {
      Project project = _project.GetProject(id);
      project.Tasks = new List<Task>();

      IEnumerable<Task> tasks = _task.GetAllTasks();
      foreach (Task task in tasks)
      {
        if (task.ProjectId == id)
        {
          task.TaskStatus = _taskStatus.GetTaskStatus(task.StatusId);
          project.Tasks.Add(task);
        }
      }

      return View(project);
    }

    [HttpGet("create")]
    public IActionResult Create(int id)
    {
      TaskViewModel taskVM = new TaskViewModel();
      taskVM.Task = new Task();
      taskVM.Task.ProjectId = id;
      taskVM.Task.StatusId = _taskStatus.GetDefaultTaskStatus();
      taskVM.Task.Deadline = DateTime.Now.AddDays(1).Date;
      taskVM.TaskStatus = _taskStatus.GetAllTaskStatus().OrderBy(ts => ts.OrderPriority).ToList();
      return View(taskVM);
    }

    [HttpGet("{taskId}/edit")]
    public ViewResult Edit(int id, int taskId)
    {
      TaskViewModel taskVM = new TaskViewModel();
      taskVM.Task = _task.GetTask(taskId);
      taskVM.TaskStatus = _taskStatus.GetAllTaskStatus().OrderBy(ts => ts.OrderPriority).ToList();

      return View(taskVM);
    }

    [HttpGet("{taskId}/delete")]
    public RedirectToActionResult Delete(int taskId)
    {
      _task.Delete(taskId);
      return RedirectToAction("index");
    }

    [HttpPost("create")]
    public IActionResult Create(Task newTask)
    {
      _task.Add(newTask);
      return RedirectToAction("Index");
    }

    [HttpPost("{taskId}/edit")]
    public IActionResult Edit(TaskViewModel taskViewModel)
    {
      _task.Update(taskViewModel.Task);
      return RedirectToAction("index");
    }
    
  }
}