using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using ProjectTracker.Models;
using ProjectTracker.Securities;
using ProjectTracker.ViewModels;

namespace ProjectTracker.Controllers
{
  public class HomeController : Controller
  {
    private readonly IProject _project;
    private readonly ITask _task;
    private readonly IMember _member;
    private readonly IDataProtector _protectMemberId;

    public HomeController(IProject project, ITask task, IMember member, IDataProtectionProvider dataProtectionProvider, DataProtectionStrings dataProtectionStrings)
    {
      _member = member;
      _task = task;
      _project = project;
      _protectMemberId = dataProtectionProvider.CreateProtector(dataProtectionStrings.MemberId);
    }
    
    public async Task<IActionResult> Index()
    {
      HomeIndexViewModel model = new HomeIndexViewModel();

      model.ProjectsCount = (await _project.GetAllProjectsAsync()).Count();
      
      model.TasksCount = (await _task.GetAllTasksAsync()).Count();

      model.Members = (await _member.GetAllMembersAsync())
        .Select(m => {
          m.EncryptedId = _protectMemberId.Protect(m.Id);
          return m;
        })
        .ToList();

      return View(model);
    }

    [HttpGet("/AccessDenied")]
    public IActionResult AccessDenied() => View();
  }
}