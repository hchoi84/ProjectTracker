using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ProjectTracker.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using TaskStatus = ProjectTracker.Models.TaskStatus;

namespace ProjectTracker.Controllers
{
  public class TaskStatusController : Controller
  {
    private readonly ITaskStatus _taskStatus;
    public TaskStatusController(ITaskStatus taskStatus)
    {
      _taskStatus = taskStatus;
    }

    public async Task<IActionResult> Index()
    {
      List<TaskStatus> allTaskStatus = await _taskStatus.GetAllTaskStatusAsync();
      return View(allTaskStatus);
    }

    public IActionResult Create() => View();

    public async Task<ViewResult> Edit(int id)
    {
      var taskStatus = await _taskStatus.GetTaskStatusAsync(id);
      return View(taskStatus);
    }

    public async Task<IActionResult> Delete(int id)
    {
      TaskStatus taskStatus = await _taskStatus.GetTaskStatusAsync(id);
      if (taskStatus.IsDefault)
      {
        ViewBag.ErrorMessage = "You are trying to delete a default Task Status. Please indicate another default Task Status before trying again";
        IEnumerable<TaskStatus> allTaskStatus = await _taskStatus.GetAllTaskStatusAsync();
        return View("Index", allTaskStatus);
      }
      await _taskStatus.DeleteAsync(id);
      return RedirectToAction("index");
    }

    [HttpPost]
    public async Task<IActionResult> Create(TaskStatus newTaskStatus)
    {
      await _taskStatus.AddAsync(newTaskStatus);
      return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Edit(TaskStatus editTaskStatus)
    {
      await _taskStatus.UpdateAsync(editTaskStatus);
      return RedirectToAction("index");
    }
  }
}