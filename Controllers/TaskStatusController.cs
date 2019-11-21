using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ProjectTracker.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace ProjectTracker.Controllers
{
  [AllowAnonymous]
  public class TaskStatusController : Controller
  {
    private readonly ITaskStatus _taskStatus;
    public TaskStatusController(ITaskStatus taskStatus)
    {
      _taskStatus = taskStatus;
    }

    public async Task<IActionResult> Index()
    {
      IEnumerable<ProjectTracker.Models.TaskStatus> allTaskStatus = await _taskStatus.GetAllTaskStatusAsync();
      allTaskStatus.OrderBy(ts => ts.OrderPriority);
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
      ProjectTracker.Models.TaskStatus taskStatus = await _taskStatus.GetTaskStatusAsync(id);
      if (taskStatus.IsDefault)
      {
        ModelState.AddModelError("IsDefault", "Please indicate another Default Task Status before deleting");
        IEnumerable<ProjectTracker.Models.TaskStatus> allTaskStatus = await _taskStatus.GetAllTaskStatusAsync();
        allTaskStatus.OrderBy(ts => ts.OrderPriority);
        return View("Index", allTaskStatus);
      }
      await _taskStatus.DeleteAsync(id);
      return RedirectToAction("index");
    }

    [HttpPost]
    public async Task<IActionResult> Create(ProjectTracker.Models.TaskStatus newTaskStatus)
    {
      await _taskStatus.AddAsync(newTaskStatus);
      return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Edit(ProjectTracker.Models.TaskStatus editTaskStatus)
    {
      await _taskStatus.UpdateAsync(editTaskStatus);
      return RedirectToAction("index");
    }
  }
}