using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectTracker.Models;
using ProjectTracker.ViewModels;

namespace ProjectTracker.Controllers
{
  public class HomeController : Controller
  {
    private readonly IProject _project;
    private readonly ITask _task;
    private readonly IMember _member;

    public HomeController(IProject project, ITask task, IMember member)
    {
      _member = member;
      _task = task;
      _project = project;
    }
    
    public async Task<IActionResult> Index()
    {
      HomeIndexViewModel model = new HomeIndexViewModel();

      model.ProjectsCount = (await _project.GetAllProjectsAsync()).Count();
      
      model.TasksCount = (await _task.GetAllTasksAsync()).Count();

      model.Members = await _member.GetAllMembersAsync();

      string userEmail = User.Identity.Name;
      ViewBag.MemberFullName = await _member.GetMemberFullNameByEmailAsync(userEmail);
      HttpContext.Session.SetString("FullName", await _member.GetMemberFullNameByEmailAsync(userEmail));

      return View(model);
    }

    [HttpGet("/AccessDenied")]
    public IActionResult AccessDenied() => View();
  }
}