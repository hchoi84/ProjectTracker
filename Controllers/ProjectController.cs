using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ProjectTracker.Models;

namespace ProjectTracker.Controllers
{
  [Route("project/{id}/tasks")]
  public class ProjectController : Controller
  {
    private readonly IProject _project;
    private readonly ITask _task;
    public ProjectController(IProject project, ITask task)
    {
      _project = project;
      _task = task;
    }

    [HttpGet("")]
    public IActionResult Index(int id)
    {
      Project project = _project.GetProject(id);
      project.Tasks = new List<ProjectTask>();

      IEnumerable<ProjectTask> tasks = _task.GetAllTasks();
      foreach (ProjectTask task in tasks)
      {
        if (task.ProjectId == id)
        {
          project.Tasks.Add(task);
        }
      }

      return View(project);
    }

    [HttpGet("create")]
    public IActionResult Create(int id)
    {
      ProjectTask task = new ProjectTask();
      task.ProjectId = id;
      task.Deadline = DateTime.Now.AddDays(1).Date;
      return View(task);
    }

    [HttpGet("{taskId}/edit")]
    public ViewResult Edit(int id, int taskId)
    {
      var task = _task.GetTask(taskId);
      return View(task);
    }

    [HttpGet("{taskId}/delete")]
    public RedirectToActionResult Delete(int taskId)
    {
      _task.Delete(taskId);
      return RedirectToAction("index");
    }

    [HttpPost("create")]
    public IActionResult Create(ProjectTask newProject)
    {
      _task.Add(newProject);
      return RedirectToAction("Index");
    }

    [HttpPost("{taskId}/edit")]
    public IActionResult Edit(ProjectTask editProject)
    {
      _task.Update(editProject);
      return RedirectToAction("index");
    }
    
  }
}