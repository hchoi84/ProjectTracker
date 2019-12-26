using Microsoft.AspNetCore.Mvc;

namespace ProjectTracker.Controllers
{
  public class HomeController : Controller
  {
    public IActionResult Index() => View();
  }
}