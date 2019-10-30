using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectTracker.Models
{
  public class TestProjectRepo : IProject
  {
    private List<Project> _projects;
    public TestProjectRepo()
    {
      _projects = new List<Project>
      {
        new Project() 
        {
          Id = 1,
          ProjectName = "Project Tracker Ver 1",
          Created = new DateTime(2019, 10, 30, 12, 03, 00),
          Updated = DateTime.Now,
          Deadline = DateTime.Now.AddMonths(1),
          Creator = "Howard Choi"
        },
        new Project() 
        {
          Id = 2,
          ProjectName = "Project Tracker Ver 2",
          Created = new DateTime(2019, 10, 30, 12, 03, 00),
          Updated = DateTime.Now,
          Deadline = DateTime.Now.AddMonths(2),
          Creator = "Howard Choi"
        },
        new Project() 
        {
          Id = 3,
          ProjectName = "Project Tracker Ver 3",
          Created = new DateTime(2019, 10, 30, 12, 03, 00),
          Updated = DateTime.Now,
          Deadline = DateTime.Now.AddMonths(3),
          Creator = "Howard Choi"
        }
      };
    }

    public Project Add(Project project)
    {
      project.Id = _projects.Max(p => p.Id) + 1;
      _projects.Add(project);
      return project;
    }

    public Project Delete(int id)
    {
      Project project = _projects.FirstOrDefault(p => p.Id == id);
      if (project != null)
      {
        _projects.Remove(project);
        return project;
      }
      return null;
    }

    public IEnumerable<Project> GetAllProjects()
    {
      return _projects;
    }

    public Project GetProject(int id)
    {
      Project project = _projects.FirstOrDefault(p => p.Id == id);
      if (project != null)
      {
        return project;
      }
      return null;
    }

    public Project Update(Project updatedProject)
    {
      Project project = _projects.FirstOrDefault(p => p.Id == updatedProject.Id);
      if (project != null)
      {
        project.ProjectName = updatedProject.ProjectName;
        project.Created = updatedProject.Created;
        project.Updated = updatedProject.Updated;
        project.Deadline = updatedProject.Deadline;
        project.Creator = updatedProject.Creator;
        return project;
      }
      return null;
    }
  }
}