using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ProjectTracker.Models;

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
      Task task = new Task();
      task.ProjectId = id;
      task.Deadline = DateTime.Now.AddDays(1).Date;
      return View(task);
    }

    [HttpGet("{taskId}/edit")]
    public ViewResult Edit(int id, int taskId)
    {
      var task = _task.GetTask(taskId);
      IEnumerable<TaskStatus> taskStatus = _taskStatus.GetAllTaskStatus();
      

      return View(task);
    }

    [HttpGet("{taskId}/delete")]
    public RedirectToActionResult Delete(int taskId)
    {
      _task.Delete(taskId);
      return RedirectToAction("index");
    }

    [HttpPost("create")]
    public IActionResult Create(Task newProject)
    {
      _task.Add(newProject);
      return RedirectToAction("Index");
    }

    [HttpPost("{taskId}/edit")]
    public IActionResult Edit(Task editProject)
    {
      _task.Update(editProject);
      return RedirectToAction("index");
    }
    
  }
}