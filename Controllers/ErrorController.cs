using Microsoft.AspNetCore.Mvc;

namespace ProjectTracker.Controllers
{
  public class ErrorController : Controller
  {
    [Route("Error/{statusCode}")]
    public IActionResult StatusCodeHandler(int statusCode)
    {
      if (statusCode == 404)
      {
        ViewBag.ErrorMessage = "Sorry, the URL you're trying to access does not exist";
      }
      
      return View("Error");
    }
  }
}