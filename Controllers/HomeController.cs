using System;
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

    [HttpGet("/project/create")]
    public IActionResult Create() => View();

    [HttpGet("/project/{id}/edit")]
    public ViewResult Edit(int id)
    {
      var project = _project.GetProject(id);
      project.Deadline = Convert.ToDateTime(project.Deadline.ToString("yyyy/MM/dd HH:mm tt"));
      return View(project);
    }

    public RedirectToActionResult Delete(int id)
    {
      _project.Delete(id);
      return RedirectToAction("index");
    }

    [HttpPost]
    public IActionResult Create(Project newProject)
    {
      Console.WriteLine("Creating Project");
      _project.Add(newProject);
      return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Edit(Project editProject)
    {
      _project.Update(editProject);
      return RedirectToAction("index");
    }
  }
}