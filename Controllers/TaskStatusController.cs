using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ProjectTracker.Models;
using System.Linq;

namespace ProjectTracker.Controllers
{
  public class TaskStatusController : Controller
  {
    private readonly ITaskStatus _taskStatus;
    public TaskStatusController(ITaskStatus taskStatus)
    {
      _taskStatus = taskStatus;
    }

    public IActionResult Index()
    {
      IEnumerable<TaskStatus> allTaskStatus = _taskStatus.GetAllTaskStatus().OrderBy(ts => ts.OrderPriority);
      return View(allTaskStatus);
    }

    public IActionResult Create() => View();

    public ViewResult Edit(int id)
    {
      var taskStatus = _taskStatus.GetTaskStatus(id);
      return View(taskStatus);
    }

    public IActionResult Delete(int id)
    {
      if (_taskStatus.GetTaskStatus(id).IsDefault)
      {
        ModelState.AddModelError("IsDefault", "Please indicate another Default Task Status before deleting");
        IEnumerable<TaskStatus> allTaskStatus = _taskStatus.GetAllTaskStatus().OrderBy(ts => ts.OrderPriority);
        return View("Index", allTaskStatus);
      }
      _taskStatus.Delete(id);
      return RedirectToAction("index");
    }

    [HttpPost]
    public IActionResult Create(TaskStatus newTaskStatus)
    {
      _taskStatus.Add(newTaskStatus);
      return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Edit(TaskStatus editTaskStatus)
    {
      _taskStatus.Update(editTaskStatus);
      return RedirectToAction("index");
    }
  }
}