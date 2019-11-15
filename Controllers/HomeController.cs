using System;
using Microsoft.AspNetCore.Mvc;
using ProjectTracker.Models;
using ProjectTracker.ViewModels;
using System.Linq;

namespace ProjectTracker.Controllers
{
  public class HomeController : Controller
  {
    private readonly IProject _project;
    private readonly IUser _user;
    public HomeController(IProject project, IUser user)
    {
      _user = user;
      _project = project;
    }

    public IActionResult Index()
    {
      var projects = _project.GetAllProjects();
      foreach (var project in projects)
      {
        project.Creator = _user.GetUser(project.UserId);
      }

      return View(projects);
    }

    [HttpGet("/project/create")]
    public IActionResult Create()
    {
      var projectVM = new ProjectViewModel();
      projectVM.users = _user.GetAllUsers().ToList();
      return View(projectVM);
    }

    [HttpGet("/project/{id}/edit")]
    public ViewResult Edit(int id)
    {
      var projectVM = new ProjectViewModel();
      projectVM.project = _project.GetProject(id);
      projectVM.users = _user.GetAllUsers().ToList();
      return View(projectVM);
    }

    public RedirectToActionResult Delete(int id)
    {
      _project.Delete(id);
      return RedirectToAction("index");
    }

    [HttpPost("/project/create")]
    public IActionResult Create(ProjectViewModel newProjectVM)
    {
      _project.Add(newProjectVM.project);
      return RedirectToAction("Index");
    }

    [HttpPost("/project/{id}/edit")]
    public IActionResult Edit(ProjectViewModel editProjectVM)
    {
      _project.Update(editProjectVM.project);
      return RedirectToAction("index");
    }
  }
}