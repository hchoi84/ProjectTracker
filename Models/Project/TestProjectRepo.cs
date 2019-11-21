// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;

// namespace ProjectTracker.Models
// {
//   public class TestProjectRepo : IProject
//   {
//     private List<Project> _projects;
//     public TestProjectRepo()
//     {
//       _projects = new List<Project>
//       {
//         new Project() 
//         {
//           Id = 1,
//           MemberId = "1",
//           ProjectName = "Project Tracker",
//           Created = new DateTime(2019, 10, 30, 12, 03, 00),
//           Updated = new DateTime(2019, 10, 31, 2, 40, 00),
//           Deadline = DateTime.Now.AddMonths(1),
//           Summary = "Lorem ipsum dolor, sit amet consectetur adipisicing elit. Quisquam doloremque eaque temporibus obcaecati, aut reprehenderit repellat reiciendis deserunt. Quas nulla corporis et cum eaque, tempora voluptatum pariatur blanditiis iusto expedita. Lorem ipsum dolor, sit amet consectetur adipisicing elit. Explicabo placeat nulla blanditiis dolores nesciunt quaerat quos asperiores eaque possimus eum."
//         },
//         new Project() 
//         {
//           Id = 2,
//           MemberId = "1",
//           ProjectName = "Golfio ChannelAdvisor API",
//           Created = new DateTime(2019, 11, 06, 11, 30, 00),
//           Updated = new DateTime(2019, 11, 06, 11, 30, 00),
//           Deadline = DateTime.Now.AddMonths(1),
//           Summary = "Allow data pulling from ChannelAdvisor and report generating without needing to login or do other manual work. Purpose is to increase productivity in other areas and reduce manual report creation."
//         }
//       };
//     }

//     public async Task<Project> AddAsync(Project newProject)
//     {
//       newProject.Id = _projects.Max(p => p.Id) + 1;
//       newProject.MemberId = newProject.MemberId;
//       newProject.Created = DateTime.Now;
//       newProject.Updated = DateTime.Now;
//       _projects.Add(newProject);
//       return newProject;
//     }

//     public async Task<Project> DeleteAsync(int id)
//     {
//       Project project = _projects.FirstOrDefault(p => p.Id == id);
//       if (project != null)
//       {
//         _projects.Remove(project);
//         return project;
//       }
//       return null;
//     }

//     public async Task<List<Project>> GetAllProjectsAsync()
//     {
//       return _projects.ToList();
//     }

//     public async Task<Project> GetProjectAsync(int id)
//     {
//       Project project = _projects.FirstOrDefault(p => p.Id == id);
//       if (project != null)
//       {
//         return project;
//       }
//       return null;
//     }

//     public async Task<Project> UpdateAsync(Project updatedProject)
//     {
//       Project project = _projects.FirstOrDefault(p => p.Id == updatedProject.Id);
//       if (project != null)
//       {
//         project.MemberId = updatedProject.MemberId;
//         project.ProjectName = updatedProject.ProjectName;
//         project.Updated = DateTime.Now;
//         project.Deadline = updatedProject.Deadline;
//         return project;
//       }
//       return null;
//     }
//   }
// }