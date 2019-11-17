using System;
using Microsoft.AspNetCore.Mvc;
using ProjectTracker.Models;
using ProjectTracker.ViewModels;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace ProjectTracker.Controllers
{
  [AllowAnonymous]
  public class HomeController : Controller
  {
    private readonly IProject _project;
    private readonly IMember _member;
    public HomeController(IProject project, IMember member)
    {
      _member = member;
      _project = project;
    }

    public IActionResult Index()
    {
      var projects = _project.GetAllProjects();
      foreach (var project in projects)
      {
        project.Member = _member.GetMember(project.MemberId);
      }

      return View(projects);
    }

    [HttpGet("/project/create")]
    public IActionResult Create()
    {
      var projectVM = new ProjectViewModel();
      projectVM.Members = _member.GetAllMembers().ToList();
      return View(projectVM);
    }

    [HttpGet("/project/{id}/edit")]
    public ViewResult Edit(int id)
    {
      var projectVM = new ProjectViewModel();
      projectVM.Project = _project.GetProject(id);
      projectVM.Members = _member.GetAllMembers().ToList();
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
      _project.Add(newProjectVM.Project);
      return RedirectToAction("Index");
    }

    [HttpPost("/project/{id}/edit")]
    public IActionResult Edit(ProjectViewModel editProjectVM)
    {
      _project.Update(editProjectVM.Project);
      return RedirectToAction("index");
    }
  }
}