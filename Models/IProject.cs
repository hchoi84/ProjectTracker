using System.Collections.Generic;

namespace ProjectTracker.Models
{
  public interface IProject
  {
    Project Add(Project project);
    Project GetProject(int id);
    IEnumerable<Project> GetAllProjects();
    Project Update(Project project);
    Project Delete(int id);
  }
}