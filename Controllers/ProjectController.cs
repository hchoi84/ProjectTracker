using Microsoft.AspNetCore.Mvc;
using ProjectTracker.Models;

namespace ProjectTracker.Controllers
{
  public class ProjectController : Controller
  {
    private readonly IProject _project;
    public ProjectController(IProject project)
    {
      _project = project;
    }

    [HttpGet]
    public IActionResult Index(int id)
    {
      Project project = _project.GetProject(id);

      return View(project);
    }
    
  }
}