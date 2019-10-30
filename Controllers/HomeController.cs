using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ProjectTracker.Models;

namespace ProjectTracker.Controllers
{
  public class HomeController : Controller
  {
    private readonly IProject _project;
    public HomeController(IProject project)
    {
      _project = project;
    }

    public IActionResult Index()
    {
      var projects = _project.GetAllProjects();
      
      return View(projects);
    }
  }
}